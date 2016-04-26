using System;
using System.Collections.Generic;

namespace Sean.World
{
    internal class WorldMap
    {


        public WorldMap()
        {
            MinX = 10000;
            MinZ = 10000;
            MapScale = 32;
            MaxX = MinX + (MapScale * 80);
            MaxZ = MinZ + (MapScale * 80);
        }
            
        public void Generate()
        {
            var size = new ArraySize(){
                minX=MinX, maxX=MaxX, minZ=MinZ, maxZ=MaxZ, 
                scale=MapScale};
            heightMap = PerlinNoise.GetIntMap(size, 3);
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


        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinZ { get; set; }
        public int MaxZ { get; set; }
        public int MapScale { get; set; }

        private Array<int> heightMap;
    }
}

