
using System;
using System.Collections.Generic;

namespace Sean.World
{
    internal class RegionMapData
    {
        public static int SizeX = 32;
        public static int SizeZ = 32;

        public RegionMapData (WorldMapData worldMapData, byte height)
        {
            _height = height;
            _worldMapData = worldMapData;

        }

        public string RenderWorldView()
        {
            //if (_height > WorldMapData.WaterLevel)
            //    return "O";
            //else
            //    return " ";
            return _height.ToString();
        }

        public void Render()
        {
            if (!created)
                CreateMap ();
            
            for (int x = 0; x < SizeX; x++) {
                System.Text.StringBuilder builder = new System.Text.StringBuilder ();
                for (int z = 0; z < SizeZ; z++) {
                    if (Cursor.RegionX == x && Cursor.RegionZ == z)
                        builder.Append ("*");
                    else
                        builder.Append (map [x] [z].Render ());
                }
                Console.WriteLine (builder.ToString ());
            }
        }

        private void CreateMap()
        {
            map =  Misc.GetEmptyArray<Cell>(SizeX, SizeZ);
            for (int x = 0; x < SizeX; x++) {
                for (int z = 0; z < SizeZ; z++) {
                    map [x] [z] = new Cell (_height);
                }
            }
        }

        public IEnumerable<byte> Serialize ()
        {
            yield return _height;
        }

        private WorldMapData _worldMapData;
        public byte Height { get { return _height; } }
        private byte _height;
        private bool created;
        private Cell [] [] map;
     }
}

