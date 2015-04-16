using System;

namespace Sean.World
{

	internal class MapCell
	{
		public MapCell()
		{
		}


	}

	internal class WorldMapData
	{
		public WorldMapData()
		{
			Octaves = 3;
			MinY = 0;
			MaxY = 100;
			SizeX = 20;
			SizeZ = 40;
		}

		public void Generate()
		{
            heightMap = PerlinNoise.GetIntMap(SizeX+1, SizeZ+1, MinY, MaxY, Octaves);
            mapCells = Misc.GetEmptyArray<int>(SizeX, SizeZ);
			for (int x = 0; x < SizeX; x++)
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				for (int z = 0; z < SizeZ; z++)
				{
                    builder.Append(heightMap[x][z] / 10);
				}
				Console.WriteLine(builder.ToString());
			}
		}

		public int Octaves { get; set; }
		public int MinY { get; set; }
		public int MaxY { get; set; }
		public int SizeX { get; set; }
		public int SizeZ { get; set; }

        private int[][] heightMap;
        private MapCell[][] mapCells;
	}

}

