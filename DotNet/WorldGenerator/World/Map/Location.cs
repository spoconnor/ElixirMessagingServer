using System;
namespace Sean.World
{
    public class Location
    {
        public Location(int x, int z)
        {
            X = x;
            Z = z;
        }
        public void MoveLeft(){ Z = Z - 1; }
        public void MoveRight(){ Z = Z + 1; }
        public void MoveDown(){ X = X + 1; }
        public void MoveUp(){ X = X - 1; }

        public int X { get; set; }
        public int Z { get; set; }
        /*
        public int WorldX { get { return X / RegionMapData.SizeX; } }
        public int WorldZ { get { return Z / RegionMapData.SizeZ; } }
        public int RegionX { get { return X % RegionMapData.SizeX; } }
        public int RegionZ { get { return Z % RegionMapData.SizeZ; } }
        */
        public bool EstimatedLocation { get; set; }
    }
}

