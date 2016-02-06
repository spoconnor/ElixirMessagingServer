using System;
using System.Collections.Generic;

namespace Sean.World
{
    internal class WorldMapData
    {
        public const int SizeX = 50;
        public const int SizeZ = 120;
        public const byte WaterLevel = 1;

        public WorldMapData()
        {
            Octaves = 3;
            MinY = 0;
            MaxY = 10;
            SizeZStretch = 6;
        }

        public void Generate()
        {
            heightMap = PerlinNoise.GetIntMap(SizeX+1, SizeZ+1, MinY, MaxY, Octaves);
            regionMapData = Misc.GetEmptyArray<RegionMapData>(SizeX, SizeZ);
            for (int x = 0; x < SizeX; x++) {
                for (int z = 0; z < SizeZ; z++) 
                {
                    double r = Math.Sqrt ((SizeX / 2 - x) * (SizeX / 2 - x) + (SizeZ / 2 - z) * (SizeZ / 2 - z) / SizeZStretch);

                    double angle = Math.Atan ((SizeZ / 2 - z) / ((SizeX / 2 - x) != 0 ? (SizeX / 2 - x) : 0.01));
                    //double a = Math.Abs(((SizeZ / 2)) / Math.Sin (angle));
                    //double b = Math.Abs(((SizeX / 2)) / Math.Cos (angle));
                    //double maxRadius = Math.Min(a,b);
                    float maxRadius = Math.Min (SizeX/2, SizeZ/2) + 5;

                    double rr = Math.Min (maxRadius, r);
                    double c = ((Math.Cos (Math.PI * rr / maxRadius) + 1) / 2);
                    regionMapData [x] [z] = new RegionMapData (this, (byte)(c*heightMap[x][z]));
                }
            }
        }

        public void Render ()
        {
            if (Cursor.IsRegionView) {
                regionMapData [Cursor.WorldX] [Cursor.WorldZ].Render ();
            }
            else
            {
                for (int x = 0; x < SizeX; x++) {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder ();
                    for (int z = 0; z < SizeZ; z++) {
                        if (Cursor.WorldX == x && Cursor.WorldZ == z)
                            builder.Append ("*");
                        else
                            builder.Append (regionMapData [x] [z].RenderWorldView ());
                    }
                    Console.WriteLine (builder.ToString ());
                }
            }
        }

        public IEnumerable<byte> Serialize()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    foreach (byte temp in regionMapData [x] [z].Serialize ()) {
                        yield return temp;
                    }
                }
            }
        }

        public int Octaves { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public int SizeZStretch { get; set; }

        private int[][] heightMap;
        private RegionMapData[][] regionMapData;
    }
}

