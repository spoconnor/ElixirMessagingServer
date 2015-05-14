using System.Collections;
using System.Diagnostics;
using OpenTK;

namespace Sean.World
{
	internal class Chunks : IEnumerable
	{
		public Chunks(int worldSizeX, int worldSizeZ)
		{
			_chunks = new Chunk[worldSizeX, worldSizeZ];
			for (var x = 0; x < worldSizeX; x++)
			{
				for (var z = 0; z < worldSizeZ; z++)
				{
					this[x, z] = new Chunk(x, z);
				}
			}
		}

		private readonly Chunk[,] _chunks;

		/// <summary>Get a chunk from the array. Based on world coords.</summary>
		public Chunk this[Coords coords]
		{
			get { return _chunks[coords.Xblock / Chunk.CHUNK_SIZE, coords.Zblock / Chunk.CHUNK_SIZE]; }
		}

		/// <summary>Get a chunk from the array. Based on world coords.</summary>
		public Chunk this[Position position]
		{
			get { return _chunks[position.X / Chunk.CHUNK_SIZE, position.Z / Chunk.CHUNK_SIZE]; }
		}

		/// <summary>Get a chunk from the array. Based on the x,z of the chunk in the world. Note these are chunk coords not block coords.</summary>
		public Chunk this[int x, int z]
		{
			get { return _chunks[x, z]; }
			private set { _chunks[x, z] = value; }
		}

		//internal uint UpdateCounter;

		public IEnumerator GetEnumerator()
		{
			return _chunks.GetEnumerator();
		}
	}
}
