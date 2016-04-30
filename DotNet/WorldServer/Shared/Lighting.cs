using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sean.World
{
	internal static class Lighting
	{
		/// <summary>
		/// Table uses a non linear scale by dropping each level to x% of the previous.
		/// Allows for wider range of 'darks'. Using the array table prevents needing to calculate these same values repeatedly.
		/// </summary>
		internal static readonly byte[] LightTable = new[] { (byte)37, (byte)43, (byte)48, (byte)55, (byte)62, (byte)71, (byte)81, (byte)92, (byte)104, (byte)118, (byte)135, (byte)153, (byte)174, (byte)197, (byte)224, (byte)255 }; //this table drops by *0.88 without the large initial drop (better for day/night cycle)
		internal const byte DARKEST_COLOR = 37;
        internal const int BRIGHTEST_SKYLIGHT_STRENGTH = 100;

		/// <summary>
		/// Check 6 adjacent neighbor blocks and propagate recursively as needed within this Chunk. This propagate is used for initial world loading only.
		/// Updates the chunk level arrays. Light map can be sky or item.
		/// </summary>
		/// <remarks>Changes here may need to be duplicated in PropagateLightDynamic</remarks>
		internal static void PropagateLightInitial(int x, int y, int z, byte lightStrength, Chunk chunk, byte[, ,] lightMap)
		{
			lightMap[x, y, z] = lightStrength;

			var lightMinusOne = (byte)Math.Max(lightStrength - 1, 0);
			var lightMinusTwo = (byte)Math.Max(lightStrength - 2, 0);

			//check top (light going up propogates half as much, seems more realistic) (only need to propagate if existing strength is less then what we want to set it to)
			if (y < Chunk.CHUNK_HEIGHT - 1 && chunk.Blocks[x, y + 1, z].IsTransparent && lightMap[x, y + 1, z] < lightMinusTwo) PropagateLightInitial(x, y + 1, z, lightMinusTwo, chunk, lightMap);

			//check bottom (only need to propagate if existing strength is less then what we want to set it to)
			if (y > 0 && chunk.Blocks[x, y - 1, z].IsTransparent && lightMap[x, y - 1, z] < lightMinusOne) PropagateLightInitial(x, y - 1, z, lightMinusOne, chunk, lightMap);

			//check left (only need to propagate if existing strength is less then what we want to set it to)
			if (x > 0 && chunk.Blocks[x - 1, y, z].IsTransparent && lightMap[x - 1, y, z] < lightMinusOne) PropagateLightInitial(x - 1, y, z, lightMinusOne, chunk, lightMap);

			//check right (only need to propagate if existing strength is less then what we want to set it to)
			if (x < Chunk.CHUNK_SIZE - 1 && chunk.Blocks[x + 1, y, z].IsTransparent && lightMap[x + 1, y, z] < lightMinusOne) PropagateLightInitial(x + 1, y, z, lightMinusOne, chunk, lightMap);

			//check back (only need to propagate if existing strength is less then what we want to set it to)
			if (z > 0 && chunk.Blocks[x, y, z - 1].IsTransparent && lightMap[x, y, z - 1] < lightMinusOne) PropagateLightInitial(x, y, z - 1, lightMinusOne, chunk, lightMap);

			//check front (only need to propagate if existing strength is less then what we want to set it to)
			if (z < Chunk.CHUNK_SIZE - 1 && chunk.Blocks[x, y, z + 1].IsTransparent && lightMap[x, y, z + 1] < lightMinusOne) PropagateLightInitial(x, y, z + 1, lightMinusOne, chunk, lightMap);
		}

		/// <summary>
		/// Pull lighting from adjacent chunks into this chunk as needed after all chunks have been initially built.
		/// Done by looping through all blocks on chunk border, look at adjacent block to determine if light needs to be "pulled" across.
		/// Propagate any cases where pulling is needed.
		/// </summary>
		internal static void InitializeCrossChunkPulling(Chunk chunk)
		{
			for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
			{
				if (chunk.Coords.Z > 0) //can skip when on this world edge
				{
                    var adjacentChunk = WorldData.WorldMap.Chunk(chunk.Coords.X, chunk.Coords.Z - 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--) //note: this used to loop from the heightmap down, however that wouldnt work with item light sources and because this is for initial pulling on edges only, it was only a tiny benefit
					{
						if (!chunk.Blocks[x, y, 0].IsTransparent) continue; //no need to pull light for non transparent blocks

						var skyLightStrength = chunk.SkyLightMapInitial[x, y, 0];
						if (skyLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentSkyLightStrength = adjacentChunk.SkyLightMapInitial[x, y, Chunk.CHUNK_SIZE - 1]; //pull light from neighbor chunk
							if (adjacentSkyLightStrength > 1 && adjacentSkyLightStrength > skyLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(x, y, 0, (byte)(adjacentSkyLightStrength - 1), chunk, chunk.SkyLightMapInitial);
							}
						}

						var itemLightStrength = chunk.ItemLightMapInitial[x, y, 0];
						if (itemLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentItemLightStrength = adjacentChunk.ItemLightMapInitial[x, y, Chunk.CHUNK_SIZE - 1]; //pull light from neighbor chunk
							if (adjacentItemLightStrength > 1 && adjacentItemLightStrength > itemLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(x, y, 0, (byte)(adjacentItemLightStrength - 1), chunk, chunk.ItemLightMapInitial);
							}
						}
					}
				}

				if (chunk.Coords.Z < WorldData.SizeInChunksZ - 1) //can skip when on this world edge
				{
                    var adjacentChunk = WorldData.WorldMap.Chunk(chunk.Coords.X, chunk.Coords.Z + 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--) //note: this used to loop from the heightmap down, however that wouldnt work with item light sources and because this is for initial pulling on edges only, it was only a tiny benefit
					{
						if (!chunk.Blocks[x, y, Chunk.CHUNK_SIZE - 1].IsTransparent) continue; //no need to pull light for non transparent blocks

						var skyLightStrength = chunk.SkyLightMapInitial[x, y, Chunk.CHUNK_SIZE - 1];
						if (skyLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentSkyLightStrength = adjacentChunk.SkyLightMapInitial[x, y, 0]; //pull light from neighbor chunk
							if (adjacentSkyLightStrength > 1 && adjacentSkyLightStrength > skyLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(x, y, Chunk.CHUNK_SIZE - 1, (byte)(adjacentSkyLightStrength - 1), chunk, chunk.SkyLightMapInitial);
							}
						}

						var itemLightStrength = chunk.ItemLightMapInitial[x, y, Chunk.CHUNK_SIZE - 1];
						if (itemLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentItemLightStrength = adjacentChunk.ItemLightMapInitial[x, y, 0]; //pull light from neighbor chunk
							if (adjacentItemLightStrength > 1 && adjacentItemLightStrength > itemLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(x, y, Chunk.CHUNK_SIZE - 1, (byte)(adjacentItemLightStrength - 1), chunk, chunk.ItemLightMapInitial);
							}
						}
					}
				}
			}

			for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
			{
				if (chunk.Coords.X > 0) //can skip when on this world edge
				{
                    var adjacentChunk = WorldData.WorldMap.Chunk(chunk.Coords.X - 1, chunk.Coords.Z);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--) //note: this used to loop from the heightmap down, however that wouldnt work with item light sources and because this is for initial pulling on edges only, it was only a tiny benefit
					{
						if (!chunk.Blocks[0, y, z].IsTransparent) continue; //no need to pull light for non transparent blocks

						var skyLightStrength = chunk.SkyLightMapInitial[0, y, z];
						if (skyLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentSkyLightStrength = adjacentChunk.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, z]; //pull light from neighbor chunk
							if (adjacentSkyLightStrength > 1 && adjacentSkyLightStrength > skyLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(0, y, z, (byte)(adjacentSkyLightStrength - 1), chunk, chunk.SkyLightMapInitial);
							}
						}

						var itemLightStrength = chunk.ItemLightMapInitial[0, y, z];
						if (itemLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentItemLightStrength = adjacentChunk.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, z];
							if (adjacentItemLightStrength > 1 && adjacentItemLightStrength > itemLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(0, y, z, (byte)(adjacentItemLightStrength - 1), chunk, chunk.ItemLightMapInitial);
							}
						}
					}
				}

				if (chunk.Coords.X < WorldData.SizeInChunksX - 1) //can skip when on this world edge
				{
                    var adjacentChunk = WorldData.WorldMap.Chunk(chunk.Coords.X + 1, chunk.Coords.Z);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--) //note: this used to loop from the heightmap down, however that wouldnt work with item light sources and because this is for initial pulling on edges only, it was only a tiny benefit
					{
						if (!chunk.Blocks[Chunk.CHUNK_SIZE - 1, y, z].IsTransparent) continue; //no need to pull light for non transparent blocks

						var skyLightStrength = chunk.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, z];
						if (skyLightStrength < 14) //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentSkyLightStrength = adjacentChunk.SkyLightMapInitial[0, y, z]; //pull light from neighbor chunk
							if (adjacentSkyLightStrength > 1 && adjacentSkyLightStrength > skyLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, z, (byte)(adjacentSkyLightStrength - 1), chunk, chunk.SkyLightMapInitial);
							}
						}

						var itemLightStrength = chunk.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, z];
						if (itemLightStrength < 14)  //no need to pull light for blocks that already have at least 14 light strength
						{
							var adjacentItemLightStrength = adjacentChunk.ItemLightMapInitial[0, y, z]; //pull light from neighbor chunk
							if (adjacentItemLightStrength > 1 && adjacentItemLightStrength > itemLightStrength - 1) //can only propagate if adjacent > 1
							{
								PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, z, (byte)(adjacentItemLightStrength - 1), chunk, chunk.ItemLightMapInitial);
							}
						}
					}
				}
			}

			#region Diagonal Corners
			//need to look over up to 4 diagonal corners to potentially pull from those chunks
			//subtract 2 from light strengths when looking diagonally because light does not spread directly diagonally
			if (chunk.Coords.X > 0)
			{
				if (chunk.Coords.Z > 0)
				{
					//check left/back diagonal
                    var chunkDiagonal = WorldData.WorldMap.Chunk(chunk.Coords.X - 1, chunk.Coords.Z - 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--)
					{
						var diagonalSkyLightStrength = chunkDiagonal.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1];
						if (diagonalSkyLightStrength > 2 && diagonalSkyLightStrength > chunk.SkyLightMapInitial[0, y, 0] - 2)
						{
							PropagateLightInitial(0, y, 0, (byte)(diagonalSkyLightStrength - 2), chunk, chunk.SkyLightMapInitial);
						}

						var diagonalItemLightStrength = chunkDiagonal.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1];
						if (diagonalItemLightStrength > 2 && diagonalItemLightStrength > chunk.ItemLightMapInitial[0, y, 0] - 2)
						{
							PropagateLightInitial(0, y, 0, (byte)(diagonalItemLightStrength - 2), chunk, chunk.ItemLightMapInitial);
						}
					}
				}
				if (chunk.Coords.Z < WorldData.SizeInChunksZ - 1)
				{
					//check left/front diagonal
                    var chunkDiagonal = WorldData.WorldMap.Chunk(chunk.Coords.X - 1, chunk.Coords.Z + 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--)
					{
						var diagonalSkyLightStrength = chunkDiagonal.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, 0];
						if (diagonalSkyLightStrength > 2 && diagonalSkyLightStrength > chunk.SkyLightMapInitial[0, y, Chunk.CHUNK_SIZE - 1] - 2)
						{
							PropagateLightInitial(0, y, Chunk.CHUNK_SIZE - 1, (byte)(diagonalSkyLightStrength - 2), chunk, chunk.SkyLightMapInitial);
						}

						var diagonalItemLightStrength = chunkDiagonal.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, 0];
						if (diagonalItemLightStrength > 2 && diagonalItemLightStrength > chunk.ItemLightMapInitial[0, y, Chunk.CHUNK_SIZE - 1] - 2)
						{
							PropagateLightInitial(0, y, Chunk.CHUNK_SIZE - 1, (byte)(diagonalItemLightStrength - 2), chunk, chunk.ItemLightMapInitial);
						}
					}
				}
			}
			if (chunk.Coords.X < WorldData.SizeInChunksX - 1)
			{
				if (chunk.Coords.Z > 0)
				{
					//check right/back diagonal
                    var chunkDiagonal = WorldData.WorldMap.Chunk(chunk.Coords.X + 1, chunk.Coords.Z - 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--)
					{
						var diagonalSkyLightStrength = chunkDiagonal.SkyLightMapInitial[0, y, Chunk.CHUNK_SIZE - 1];
						if (diagonalSkyLightStrength > 2 && diagonalSkyLightStrength > chunk.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, 0] - 2)
						{
							PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, 0, (byte)(diagonalSkyLightStrength - 2), chunk, chunk.SkyLightMapInitial);
						}

						var diagonalItemLightStrength = chunkDiagonal.ItemLightMapInitial[0, y, Chunk.CHUNK_SIZE - 1];
						if (diagonalItemLightStrength > 2 && diagonalItemLightStrength > chunk.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, 0] - 2)
						{
							PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, 0, (byte)(diagonalItemLightStrength - 2), chunk, chunk.ItemLightMapInitial);
						}
					}
				}
				if (chunk.Coords.Z < WorldData.SizeInChunksZ - 1)
				{
					//check right/front diagonal
                    var chunkDiagonal = WorldData.WorldMap.Chunk(chunk.Coords.X + 1, chunk.Coords.Z + 1);
					for (int y = Chunk.CHUNK_HEIGHT - 1; y > 0; y--)
					{
						var diagonalSkyLightStrength = chunkDiagonal.SkyLightMapInitial[0, y, 0];
						if (diagonalSkyLightStrength > 2 && diagonalSkyLightStrength > chunk.SkyLightMapInitial[Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1] - 2)
						{
							PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1, (byte)(diagonalSkyLightStrength - 2), chunk, chunk.SkyLightMapInitial);
						}

						var diagonalItemLightStrength = chunkDiagonal.ItemLightMapInitial[0, y, 0];
						if (diagonalItemLightStrength > 2 && diagonalItemLightStrength > chunk.ItemLightMapInitial[Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1] - 2)
						{
							PropagateLightInitial(Chunk.CHUNK_SIZE - 1, y, Chunk.CHUNK_SIZE - 1, (byte)(diagonalItemLightStrength - 2), chunk, chunk.ItemLightMapInitial);
						}
					}
				}
			}
			#endregion

            /*
			//at this point the lighting in this chunk is finished, so add to gigantic world light map
			//-couldnt do it before now because lighting isnt finalized until the cross chunk pulling is done
			//-this is essentially a copy of an array into a larger array
			for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
			{
				int worldX = chunk.Coords.WorldCoordsX + x;
				for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
				{
					int worldZ = chunk.Coords.WorldCoordsZ + z;
					for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
					{
						WorldData.SkyLightMap[worldX, y, worldZ] = chunk.SkyLightMapInitial[x, y, z];
						WorldData.ItemLightMap[worldX, y, worldZ] = chunk.ItemLightMapInitial[x, y, z];
					}
				}
			}
            */
		}
	}
}
