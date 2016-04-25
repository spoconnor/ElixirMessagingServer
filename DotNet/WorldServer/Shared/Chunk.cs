using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sean.World
{
	public class Chunk
	{
		#region Constructors
		internal Chunk(ChunkCoords chunkCoords)
		{
            Coords = chunkCoords;
			Blocks = new Blocks(CHUNK_SIZE, CHUNK_HEIGHT, CHUNK_SIZE);
            HeightMap = new Array<int>(CHUNK_SIZE, CHUNK_SIZE);
			//Clutters = new HashSet<Clutter>();
			LightSources = new ConcurrentDictionary<int, LightSource>();
			//Mobs = new HashSet<Mob>();
			GameItems = new ConcurrentDictionary<int, GameItemDynamic>();
		}
		#endregion

		#region Properties
		public const int CHUNK_SIZE = 32;
		public const int CHUNK_HEIGHT = 96;
		public const int SIZE_IN_BYTES = CHUNK_SIZE * CHUNK_HEIGHT * CHUNK_SIZE * sizeof(ushort);
		private const int CLUTTER_RENDER_DISTANCE = CHUNK_SIZE * 4;
		private const int GAME_ITEM_RENDER_DISTANCE = CLUTTER_RENDER_DISTANCE;

		public ChunkCoords Coords;
		public Blocks Blocks;

		/// <summary>Heighest level in each vertical column containing a non transparent block. Sky light does not shine through this point. Used in rendering and lighting calculations.</summary>
		public Array<int> HeightMap;
		public byte[,,] SkyLightMapInitial;
		public byte[,,] ItemLightMapInitial;

		/// <summary>Clutter contained in this chunk. Clutter can be stored at the chunk level only because it can never move off the chunk.</summary>
		/// <remarks>HashSet because currently Clutter cannot be added outside of initial world generation. Collection is locked during removal.</remarks>
		//internal HashSet<Clutter> Clutters;

		/// <summary>
		/// Light sources contained in this chunk. Light sources can be stored at the chunk level only because they can never move off the chunk.
		/// TBD: when a light source is destroyed, does it become a GameItem?
		/// </summary>
		internal ConcurrentDictionary<int, LightSource> LightSources;

		//internal HashSet<Mob> Mobs; //also stored at World level in ConcurrentDictionary
		
		internal ConcurrentDictionary<int, GameItemDynamic> GameItems; //also stored at World level

		/// <summary>Distance of the chunk from the player in number of blocks.</summary>
        public double DistanceFromPlayer(Coords coords)
		{
			return Math.Sqrt(Math.Pow(coords.Xf - Coords.WorldCoordsX, 2) + Math.Pow(coords.Zf - Coords.WorldCoordsZ, 2));
		}
		
		/// <summary>Lookup for the Chunk Vbo containing the position, normal and texCoords Vbo's for this chunk and texture type.</summary>
		//private readonly ChunkVbo[] _chunkVbos = new ChunkVbo[Enum.GetNames(typeof(BlockTextureType)).Length];

		/// <summary>Total number of vbo's being rendered for blocks in this chunk.</summary>
		//internal int VboCount { get { return _chunkVbos.Count(chunkVbo => chunkVbo != null); } }

		/// <summary>Total number of primitives being rendered for blocks in this chunk.</summary>
		//internal int PrimitiveCount { get { return _chunkVbos.Where(chunkVbo => chunkVbo != null).Sum(chunkVbo => chunkVbo.PrimitiveCount); } }

		/// <summary>
		/// The build state of this chunk. When a chunk gets built it is set to 'Built' state and then marked dirty so the vbo will then get created/recreated.
		/// When a change happens in the chunk, its build state is set to 'Queued' for it to get rebuilt. When loading the initial chunk frustum, chunks are
		/// set to QueuedInitialFrustum because they dont need to be pushed to the ChangedChunkQueue. Chunks that should be built in order in the distance are
		/// set to QueuedFar and placed on the FarChunkQueue.
		/// </summary>
		internal enum BuildState : byte
		{
			/// <summary>Chunk is not loaded.</summary>
			NotLoaded,
			/// <summary>Chunk is queued for build. It will be on the ChangedChunkQueue.</summary>
			Queued,
			/// <summary>Chunk is queued for build in the distance. It will be on the FarChunkQueue.</summary>
			QueuedFar,
			/// <summary>
			/// Chunk is queued for build as part of day/night lighting cycle. It will be on the FarChunkQueue.
			/// Useful because we can determine the reason the chunk is on the far queue.
			/// -this status could eventually have logic to just do light calcs and rebuffer stored arrays if we decide to store them
			/// </summary>
			QueuedDayNight,
			/// <summary>Chunk is queued for build as part of the initial frustum of chunks loaded before entering the world.</summary>
			QueuedInitialFrustum,
			/// <summary>Chunk is queued for build as part of the initial set of chunks outside the initial radius after entering the world.</summary>
			QueuedInitialFar,
			/// <summary>Chunk is currently building.</summary>
			Building,
			/// <summary>Chunk is built.</summary>
			Built
		}

        /*
		private volatile BuildState _chunkBuildState = BuildState.NotLoaded;
		internal BuildState ChunkBuildState
		{
			get { return _chunkBuildState; }
			set
			{
				_chunkBuildState = value;
				switch (value)
				{
					case BuildState.Queued:
						WorldHost.ChangedChunkQueue.Enqueue(this);
						WorldHost.BuildChunkHandle.Set();
						break;
					case BuildState.QueuedDayNight:
					case BuildState.QueuedFar:
					case BuildState.QueuedInitialFar:
						WorldHost.FarChunkQueue.Enqueue(this);
						WorldHost.BuildChunkHandle.Set();
						break;
					case BuildState.Built:
						if (ChunkBufferState == BufferState.VboBuffered) ChunkBufferState = BufferState.VboDirty;
						break;
					case BuildState.NotLoaded:
						ChunkBufferState = BufferState.VboNotBuffered;
						UnloadData();
						break;
				}
			}
		}*/

		/// <summary>
		/// The buffer state of this chunk. Refers to whether a vbo is created 'VboBuffered', needs to be created or recreated 'VboDirty' or has not yet been buffered 'VboNotBuffered'.
		/// The reason the buffer state and build state are different enums is because the chunk needs to wait to be 'Built' before it can be buffered to a vbo.
		/// </summary>
		internal enum BufferState { VboNotBuffered, VboDirty, VboBuffered }
		internal volatile BufferState ChunkBufferState = BufferState.VboNotBuffered;
		#endregion

		#region Height Map
		/// <summary>Y level of the deepest transparent block in this chunk. When building the vbo, we only need to start at 1 level below this.</summary>
		internal int DeepestTransparentLevel { get; set; }

		/// <summary>Y level of the highest non air block. Improves chunk build times. Nothing is rendered higher then this so when building the chunk vbo theres no need to go any higher.</summary>
		internal int HighestNonAirLevel { get; set; }

		/// <summary>
		/// Build a heightmap for this chunk. This is the highest non transparent block in each vertical column.
		/// Leaves, water and other transparent blocks that light can shine through do not count.
		/// </summary>
		/// <remarks>The height map is used for lighting. Its also used to determine the players starting Y position.</remarks>
		internal void BuildHeightMap()
		{
			DeepestTransparentLevel = CHUNK_HEIGHT; //initialize to top of chunk until this gets calculated
			HighestNonAirLevel = 0; //initialize to bottom of chunk until this gets calculated
			for (var x = 0; x < CHUNK_SIZE; x++)
			{
				for (var z = 0; z < CHUNK_SIZE; z++)
				{
					for (var y = CHUNK_HEIGHT - 1; y >= 0; y--) //loop from the highest block position downward until we find a solid block
					{
						var block = Blocks[x, y, z];
						if (y > HighestNonAirLevel && block.Type != Block.BlockType.Air) HighestNonAirLevel = y;
						if (block.IsTransparent) continue;
						HeightMap[x, z] = y;
						break;
					}

					for (var y = 0; y < CHUNK_HEIGHT - 1; y++) //loop from the base of the world upwards until finding a transparent block
					{
						if (!Blocks[x, y, z].IsTransparent) continue;
						if (y < DeepestTransparentLevel) DeepestTransparentLevel = y; //record this as the deepest transparent level if it is deeper then what we had previously
						break;
					}
				}
			}
		}

		/// <summary>Updates the heightmap following a block placement. Usually a lot quicker then re-building the heightmap.</summary>
		internal void UpdateHeightMap(ref Block block, int chunkRelativeX, int yLevel, int chunkRelativeZ)
		{
			var currentHeight = HeightMap[chunkRelativeX, chunkRelativeZ];
			if (block.IsTransparent) //transparent block
			{
				//update height map
				if (yLevel == currentHeight)
				{
					//transparent block being placed at the previous heightmap level, most likely removing a block (which places Air), so we need to find the next non transparent block for the heightmap
					for (var y = currentHeight - 1; y >= 0; y--) //start looking down from the previous heightmap level
					{
						if (y > 0 && Blocks[chunkRelativeX, y, chunkRelativeZ].IsTransparent) continue;
						//found the next non transparent block, update the heightmap and exit
						HeightMap[chunkRelativeX, chunkRelativeZ] = y;
						break;
					}
				}

				//update deepest transparent level
				if (yLevel < DeepestTransparentLevel) DeepestTransparentLevel = yLevel;
			}
			else //non transparent block
			{
				//update height map
				//when placing a non transparent block, check if its above the current heightmap value and if so update the heightmap
				if (yLevel > currentHeight) HeightMap[chunkRelativeX, chunkRelativeZ] = yLevel;

				//update deepest transparent level
				if (yLevel == DeepestTransparentLevel)
				{
					//this block is being set at the DeepestTransparentLevel of this chunk
					//we will need to calc if this is still the deepest level (because theres another transparent block at this depth) or what the new level is
					//the easiest way to do that is just rebuild the height map, even though all we really need to do is the portion that updates the deepest level
					BuildHeightMap();
					return; //no need to continue on to check anything else when doing a full heightmap rebuild
				}
			}

			//update HighestNonAirLevel property
			//1. if placing air (removing block), is it at same level as previous HighestNonAir?, just rebuild HeightMap in this case, otherwise do nothing
			//2. if placing anything other then air, simply check if its > HighestNonAirLevel and set it
			if (block.Type == Block.BlockType.Air) //removing a block
			{
				if (yLevel == HighestNonAirLevel) BuildHeightMap();
			}
			else //adding a block
			{
				if (yLevel > HighestNonAirLevel) HighestNonAirLevel = yLevel;
			}
		}
		#endregion

		/*
		#region Frustum
		// Shortest Face height. Used in frustum checks. Calculated while building vbo's
		// Use for frustum logic instead of deepestTransparentLevel because this will also
		// account for faces drawn below the DeepestTransparentLevel to be visible from adjacent chunks
		//private int _shortestFaceHeight; // TODO - not calculated

		/// <summary>Is this chunk in the players view frustum.</summary>
		/// <seealso cref="http://www.crownandcutlass.com/features/technicaldetails/frustum.html"/>
        internal bool IsInFrustum(Vector4d nearFrustum, Vector4d farFrustum, Vector4d leftFrustum, Vector4d rightFrustum, Vector4d topFrustum, Vector4d bottomFrustum)
		{
				float minX = Coords.WorldCoordsX;
				var maxX = minX + CHUNK_SIZE;
				float minZ = Coords.WorldCoordsZ;
				var maxZ = minZ + CHUNK_SIZE;

				var nfXmin = nearFrustum.X * minX;
                var nfXmax = nearFrustum.X * maxX;
                var nfYmin = nearFrustum.Y * _shortestFaceHeight;
                var nfYmax = nearFrustum.Y * HighestNonAirLevel;
                var nfZmin = nearFrustum.Z * minZ;
                var nfZmax = nearFrustum.Z * maxZ;

                if (nfXmin + nfYmax + nfZmin + nearFrustum.W < -0.005f
                    && nfXmin + nfYmax + nfZmax + nearFrustum.W < -0.005f
                    && nfXmax + nfYmax + nfZmin + nearFrustum.W < -0.005f
                    && nfXmax + nfYmax + nfZmax + nearFrustum.W < -0.005f
                    && nfXmin + nfYmin + nfZmin + nearFrustum.W < -0.005f
                    && nfXmin + nfYmin + nfZmax + nearFrustum.W < -0.005f
                    && nfXmax + nfYmin + nfZmin + nearFrustum.W < -0.005f
                    && nfXmax + nfYmin + nfZmax + nearFrustum.W < -0.005f) return false;

                var ffXmin = farFrustum.X * minX;
                var ffXmax = farFrustum.X * maxX;
                var ffYmin = farFrustum.Y * _shortestFaceHeight;
                var ffYmax = farFrustum.Y * HighestNonAirLevel;
                var ffZmin = farFrustum.Z * minZ;
                var ffZmax = farFrustum.Z * maxZ;

                if (ffXmin + ffYmax + ffZmin + farFrustum.W < -0.005f
                    && ffXmin + ffYmax + ffZmax + farFrustum.W < -0.005f
                    && ffXmax + ffYmax + ffZmin + farFrustum.W < -0.005f
                    && ffXmax + ffYmax + ffZmax + farFrustum.W < -0.005f
                    && ffXmin + ffYmin + ffZmin + farFrustum.W < -0.005f
                    && ffXmin + ffYmin + ffZmax + farFrustum.W < -0.005f
                    && ffXmax + ffYmin + ffZmin + farFrustum.W < -0.005f
                    && ffXmax + ffYmin + ffZmax + farFrustum.W < -0.005f) return false;

				var lfXmin = leftFrustum.X * minX;
                var lfXmax = leftFrustum.X * maxX;
                var lfYmin = leftFrustum.Y * _shortestFaceHeight;
                var lfYmax = leftFrustum.Y * HighestNonAirLevel;
                var lfZmin = leftFrustum.Z * minZ;
                var lfZmax = leftFrustum.Z * maxZ;

                if (lfXmin + lfYmax + lfZmin + leftFrustum.W < -0.005f
                    && lfXmin + lfYmax + lfZmax + leftFrustum.W < -0.005f
                    && lfXmax + lfYmax + lfZmin + leftFrustum.W < -0.005f
                    && lfXmax + lfYmax + lfZmax + leftFrustum.W < -0.005f
                    && lfXmin + lfYmin + lfZmin + leftFrustum.W < -0.005f
                    && lfXmin + lfYmin + lfZmax + leftFrustum.W < -0.005f
                    && lfXmax + lfYmin + lfZmin + leftFrustum.W < -0.005f
                    && lfXmax + lfYmin + lfZmax + leftFrustum.W < -0.005f) return false;

				var rfXmin = rightFrustum.X * minX;
				var rfXmax = rightFrustum.X * maxX;
				var rfYmin = rightFrustum.Y * _shortestFaceHeight;
				var rfYmax = rightFrustum.Y * HighestNonAirLevel;
				var rfZmin = rightFrustum.Z * minZ;
				var rfZmax = rightFrustum.Z * maxZ;

				if (rfXmin + rfYmax + rfZmin + rightFrustum.W < -0.005f
					&& rfXmin + rfYmax + rfZmax + rightFrustum.W < -0.005f
					&& rfXmax + rfYmax + rfZmin + rightFrustum.W < -0.005f
					&& rfXmax + rfYmax + rfZmax + rightFrustum.W < -0.005f
					&& rfXmin + rfYmin + rfZmin + rightFrustum.W < -0.005f
					&& rfXmin + rfYmin + rfZmax + rightFrustum.W < -0.005f
					&& rfXmax + rfYmin + rfZmin + rightFrustum.W < -0.005f
					&& rfXmax + rfYmin + rfZmax + rightFrustum.W < -0.005f) return false;

				var tfXmin = topFrustum.X * minX;
				var tfXmax = topFrustum.X * maxX;
				var tfYmin = topFrustum.Y * _shortestFaceHeight;
				var tfYmax = topFrustum.Y * HighestNonAirLevel;
				var tfZmin = topFrustum.Z * minZ;
				var tfZmax = topFrustum.Z * maxZ;

				if (tfXmin + tfYmax + tfZmin + topFrustum.W < -0.005f
					&& tfXmin + tfYmax + tfZmax + topFrustum.W < -0.005f
					&& tfXmax + tfYmax + tfZmin + topFrustum.W < -0.005f
					&& tfXmax + tfYmax + tfZmax + topFrustum.W < -0.005f
					&& tfXmin + tfYmin + tfZmin + topFrustum.W < -0.005f
					&& tfXmin + tfYmin + tfZmax + topFrustum.W < -0.005f
					&& tfXmax + tfYmin + tfZmin + topFrustum.W < -0.005f
					&& tfXmax + tfYmin + tfZmax + topFrustum.W < -0.005f) return false;

				var bfXmin = bottomFrustum.X * minX;
				var bfXmax = bottomFrustum.X * maxX;
				var bfYmin = bottomFrustum.Y * _shortestFaceHeight;
				var bfYmax = bottomFrustum.Y * HighestNonAirLevel;
				var bfZmin = bottomFrustum.Z * minZ;
				var bfZmax = bottomFrustum.Z * maxZ;

				if (bfXmin + bfYmax + bfZmin + bottomFrustum.W < -0.005f
					&& bfXmin + bfYmax + bfZmax + bottomFrustum.W < -0.005f
					&& bfXmax + bfYmax + bfZmin + bottomFrustum.W < -0.005f
					&& bfXmax + bfYmax + bfZmax + bottomFrustum.W < -0.005f
					&& bfXmin + bfYmin + bfZmin + bottomFrustum.W < -0.005f
					&& bfXmin + bfYmin + bfZmax + bottomFrustum.W < -0.005f
					&& bfXmax + bfYmin + bfZmin + bottomFrustum.W < -0.005f
					&& bfXmax + bfYmin + bfZmax + bottomFrustum.W < -0.005f) return false;

				return true;
		}
		#endregion
		*/

        private const int UPDATES_PER_SECOND = 1;
        private const int CHUNK_UPDATE_INTERVAL = 1;
		internal bool WaterExpanding { get; set; }
        private const int WATER_UPDATE_INTERVAL = (int)(UPDATES_PER_SECOND * 1.5) / CHUNK_UPDATE_INTERVAL; //1.5s
		/// <summary>Only called for SinglePlayer and Servers.</summary>
		private void WaterExpand()
		{
			Debug.WriteLine("Water expanding in chunk {0}...", Coords);
			var newWater = new List<Position>();
			for (var i = 0; i < CHUNK_SIZE; i++)
			{
				for (var j = 0; j < CHUNK_HEIGHT; j++)
				{
					for (var k = 0; k < CHUNK_SIZE; k++)
					{
						if (Blocks[i, j, k].Type != Block.BlockType.Water) continue;
						var belowCurrent = new Position();
						for (var q = 0; q < 5; q++)
						{
							Position adjacent;
							switch (q)
							{
								case 0:
									adjacent = new Position(Coords.WorldCoordsX + i, j - 1, Coords.WorldCoordsZ + k);
									belowCurrent = adjacent;
									break;
								case 1:
									adjacent = new Position(Coords.WorldCoordsX + i + 1, j, Coords.WorldCoordsZ + k);
									break;
								case 2:
									adjacent = new Position(Coords.WorldCoordsX + i - 1, j, Coords.WorldCoordsZ + k);
									break;
								case 3:
									adjacent = new Position(Coords.WorldCoordsX + i, j, Coords.WorldCoordsZ + k + 1);
									break;
								default:
									adjacent = new Position(Coords.WorldCoordsX + i, j, Coords.WorldCoordsZ + k - 1);
									break;
							}

							if (newWater.Contains(adjacent)) continue;

							//if there's air or water below the current block, don't spread sideways
							if (q != 0 && belowCurrent.IsValidBlockLocation && (Blocks[belowCurrent].Type == Block.BlockType.Air || Blocks[belowCurrent].Type == Block.BlockType.Water)) continue;
							if (adjacent.IsValidBlockLocation && adjacent.GetBlock().Type == Block.BlockType.Air) newWater.Add(adjacent);
						}
					}
				}
			}

			if (newWater.Count == 0)
			{
				WaterExpanding = false;
				Debug.WriteLine("Water finished expanding in chunk {0}", Coords);
				return;
			}

			var addBlocks = new List<AddBlock>();
			Settings.ChunkUpdatesDisabled = true; //change blocks while updates are disabled so chunk is only rebuilt once
			foreach (var newWaterPosition in newWater.Where(newWaterCoords => newWaterCoords.GetBlock().Type != Block.BlockType.Water))
			{
				WorldData.PlaceBlock(newWaterPosition, Block.BlockType.Water);
			    var temp = newWaterPosition;
				addBlocks.Add(new AddBlock(ref temp, Block.BlockType.Water));
			}
			Settings.ChunkUpdatesDisabled = false;

			if (addBlocks.Count > 0)
			{
// TODO - send to all players
/*
				foreach (var player in Server.Controller.Players.Values)
				{
					var addBlockMulti = new AddBlockMulti {ConnectedPlayer = player};
					addBlockMulti.Blocks.AddRange(addBlocks);
					addBlockMulti.Send();
				}
*/
			}
		}

		internal bool GrassGrowing { get; set; }
		private const int GRASS_UPDATE_INTERVAL = UPDATES_PER_SECOND * 75 / CHUNK_UPDATE_INTERVAL; //75s
		//private readonly int _grassOffset = Settings.Random.Next(0, GRASS_UPDATE_INTERVAL); //stagger grass growth randomly for each chunk
		/// <summary>Only called for SinglePlayer and Servers.</summary>
		private void GrassGrow()
		{
			var possibleChanges = new List<Tuple<Block.BlockType, Position>>();
			for (var x = 0; x < CHUNK_SIZE; x++)
			{
				int worldX = Coords.WorldCoordsX + x;
				for (var z = 0; z < CHUNK_SIZE; z++)
				{
					int worldZ = Coords.WorldCoordsZ + z;
					for (var y = 0; y <= Math.Min(CHUNK_HEIGHT - 1, HeightMap[x, z] + 1); y++) //look +1 above heightmap as water directly above heightmap could change to ice
					{
						var blockType = Blocks[x, y, z].Type;
						switch (blockType)
						{
							case Block.BlockType.Grass:
							case Block.BlockType.Dirt:
							case Block.BlockType.Snow:
							case Block.BlockType.Water:
							case Block.BlockType.Sand:
							case Block.BlockType.SandDark:
								break;
							default:
								continue; //continue if this block type can never cause changes
						}

						bool hasAirAbove = y >= CHUNK_HEIGHT - 1 || Blocks[x, y + 1, z].Type == Block.BlockType.Air;
						bool isReceivingSunlight = y > HeightMap[x, z] || (hasAirAbove && WorldData.HasAdjacentBlockReceivingDirectSunlight(worldX, y, worldZ));

						switch (WorldData.WorldType)
						{
							case WorldType.Grass:
								if (isReceivingSunlight)
								{
									switch (blockType)
									{
										case Block.BlockType.Dirt:
											if (hasAirAbove) possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Grass, new Position(worldX, y, worldZ)));
											continue;
										case Block.BlockType.Snow:
											possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Dirt, new Position(worldX, y, worldZ)));
											continue;
									}
								}
								else
								{
									switch (blockType)
									{
										case Block.BlockType.Grass:
										case Block.BlockType.Snow:
											possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Dirt, new Position(worldX, y, worldZ)));
											continue;
									}
								}
								break;
							case WorldType.Desert: //lighting doesnt matter for deserts
								switch (blockType)
								{
									case Block.BlockType.Grass:
									case Block.BlockType.Snow:
										possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Sand, new Position(worldX, y, worldZ)));
										continue;
									case Block.BlockType.SandDark:
										if (hasAirAbove) possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Sand, new Position(worldX, y, worldZ)));
										continue;
								}
								break;
							case WorldType.Winter:
								switch (blockType)
								{
									case Block.BlockType.Water:
										//water with air above and without more water below can freeze
										//note: this will cause multiple lightbox updates and chunk queues if multiple water freezes at once because water -> ice is a change in transparency; therefore this is acceptable
										if (hasAirAbove)
										{
											var hasWaterBelow = y > 0 && Blocks[x, y - 1, z].Type == Block.BlockType.Water;
											if (!hasWaterBelow) possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Ice, new Position(worldX, y, worldZ)));
										}
										continue;
								}
								
								if (isReceivingSunlight)
								{
									switch (blockType)
									{
										case Block.BlockType.Dirt:
											if (hasAirAbove) possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Snow, new Position(worldX, y, worldZ)));
											continue;
										case Block.BlockType.Grass:
											possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Snow, new Position(worldX, y, worldZ)));
											continue;
									}
								}
								else
								{
									switch (blockType)
									{
										case Block.BlockType.Grass:
										case Block.BlockType.Snow:
											possibleChanges.Add(new Tuple<Block.BlockType, Position>(Block.BlockType.Dirt, new Position(worldX, y, worldZ)));
											continue;
									}
								}
								break;
						}
					}
				}
			}

			if (possibleChanges.Count == 0)
			{
				//this happens after a change is made in the chunk that did not cause any possible grass grow style changes
				GrassGrowing = false;
				Debug.WriteLine("Grass finished growing in chunk {0} No possible changes found", Coords);
				return;
			}
			Debug.WriteLine("Grass growing in chunk {0} {1} possible change(s)", Coords, possibleChanges.Count);

			var changesMade = 0;
			var addBlocks = new List<AddBlock>(); //only gets used for servers
			Settings.ChunkUpdatesDisabled = true; //change blocks while updates are disabled so chunk is only rebuilt once
			{
				foreach (var change in possibleChanges)
				{
					//add some randomness so the changes dont happen all at once
					if (possibleChanges.Count > 1)
					{
						switch (change.Item1) //can assign different percentages based on block type
						{
							case Block.BlockType.Ice:
								if (Settings.Random.NextDouble() > 0.05) continue; //give ice forming a very low chance because its a change in transparency and causes lightbox updates and must queue multiple chunks
								break;
							default:
								if (Settings.Random.NextDouble() > 0.18) continue;
								break;
						}
					}
					else //when only one possible change is left, greatly increase its chance; prevents tons of chunks lingering performing the logic until the final change gets made
					{
						if (Settings.Random.NextDouble() > 0.5) continue;
					}

					changesMade++;
					var changePosition = change.Item2;
					WorldData.PlaceBlock(changePosition, change.Item1);
					addBlocks.Add(new AddBlock(ref changePosition, change.Item1));
				}
			}
			Settings.ChunkUpdatesDisabled = false;

			//send updates to multiplayer clients
			if (addBlocks.Count > 0)
			{
// TODO - send to all players
/*
				foreach (var player in Server.Controller.Players.Values)
				{
					var addBlockMulti = new AddBlockMulti {ConnectedPlayer = player};
					addBlockMulti.Blocks.AddRange(addBlocks);
					addBlockMulti.Send();
				}
*/
			}

			if (changesMade == possibleChanges.Count)
			{
				//when all possible changes have been made we can stop GrassGrowing here without waiting for the next iteration to confirm it
				GrassGrowing = false;
				Debug.WriteLine("Grass finished growing in chunk {0} All possible changes made", Coords);
			}
		}
	
        #region Xml
		internal XmlNode GetXml(XmlDocument xmlDocument)
		{
			var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "C", string.Empty);
			if (xmlNode.Attributes == null) throw new Exception("Node attributes is null.");
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("X")).Value = Coords.X.ToString();
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("Z")).Value = Coords.Z.ToString();
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("WaterExpanding")).Value = WaterExpanding.ToString();
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("GrassGrowing")).Value = GrassGrowing.ToString();
			return xmlNode;
		}
		#endregion
	}
}
