using System;
using System.Collections.Generic;

namespace Sean.World
{
    internal class WorldMap
    {
        public const byte WaterLevel = 1;
        public const int RegionSize = 5;
        public const int MapX = 120;
        public const int MapZ = 60;

        public WorldMap()
        {
            Octaves = 3;
            MinY = 0;
            MaxY = 10;
            SizeZStretch = 6;
        }
            
        public void Generate()
        {
            var size = new ArraySize(){minX=0, maxX=MapX, minZ=0, maxZ=MapZ, scale=RegionSize};
            heightMap = PerlinNoise.GetIntMap(size, 0,MapX,0,MapZ, 3);
        }

        /*
        public void Render ()
        {
            for (int x = 0; x < MapX; x++) {
                System.Text.StringBuilder builder = new System.Text.StringBuilder ();
                for (int z = 0; z < MapZ; z++) {
                    if (Cursor.WorldX == x && Cursor.WorldZ == z)
                        builder.Append ("*");
                    else
                        builder.Append (chunks [x,z].Render ());
                }
                Console.WriteLine (builder.ToString ());
            }
        }
        */


        public int Octaves { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public int SizeZStretch { get; set; }

        private Array<int> heightMap;
        private Chunks chunks;
    }
}

