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
            if (_height > WorldMapData.WaterLevel)
                return ".";
            else
                return " ";
        }

        private byte _height;
    }
}

