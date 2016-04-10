using System;
namespace Sean.World
{
    public class Cell
    {
        public Cell (byte height)
        {
            _height = height;
        }

        public string Render()
        {
            //if (_height > WorldMapData.WaterLevel)
            //    return ".";
            //else
            //    return " ";
            return _height.ToString ();
        }

        private byte _height;
    }
}

