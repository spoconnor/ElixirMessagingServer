using System;
using System.Collections.Generic;

namespace Sean.World
{
    internal class MapCell
    {
        private Dictionary<byte, byte> cells = new Dictionary<byte, byte>();
        private byte minHeight = 0;
        private byte maxHeight = 0;

        public MapCell()
        {
        }

        public void Generate(byte heightMap)
        {
            maxHeight = heightMap;
            for (byte y=0; y<heightMap; y++) {
                cells [y] = 5;
            }
            cells [maxHeight] = 5;
        }

        public IEnumerable<byte> Serialize()
        {
            yield return minHeight;
            yield return maxHeight;
            for (byte y=minHeight; y<=maxHeight; y++) {
                yield return cells [y];
            }
        }
    }

    internal class WorldMapData
    {
        public WorldMapData()
        {
            Octaves = 3;
            MinY = 0;
            MaxY = 10;
            SizeX = 10;
            SizeZ = 10;
        }

        public void Generate()
        {
            heightMap = PerlinNoise.GetIntMap(SizeX+1, SizeZ+1, MinY, MaxY, Octaves);
            mapCells = Misc.GetEmptyArray<MapCell>(SizeX, SizeZ);
            for (int x = 0; x < SizeX; x++)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int z = 0; z < SizeZ; z++)
                {
                    builder.Append(heightMap[x][z]);
                    mapCells [x] [z] = new MapCell ();
                    mapCells [x] [z].Generate ((byte)heightMap[x][z]);
                }
                Console.WriteLine(builder.ToString());
            }
        }

        public IEnumerable<byte> Serialize()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    foreach (byte temp in mapCells [x] [z].Serialize ()) {
                        yield return temp;
                    }
                }
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

