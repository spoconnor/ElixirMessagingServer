using System;
namespace Sean.World
{
    public static class Cursor
    {
        public static void MoveLeft(){ Z = Z - (IsRegionView ? 1 : RegionMapData.SizeZ); }
        public static void MoveRight(){ Z = Z + (IsRegionView ? 1 : RegionMapData.SizeZ); }
        public static void MoveDown(){ X = X + (IsRegionView ? 1 : RegionMapData.SizeX); }
        public static void MoveUp(){ X = X - (IsRegionView ? 1 : RegionMapData.SizeX); }
        public static void ZoomIn () { IsRegionView = true; }
        public static void ZoomOut () { IsRegionView = false; }

        public static int X { get; set; }
        public static int Z { get; set; }
        public static int WorldX { get { return X / RegionMapData.SizeX; } }
        public static int WorldZ { get { return Z / RegionMapData.SizeZ; } }
        public static int RegionX { get { return X % RegionMapData.SizeX; } }
        public static int RegionZ { get { return Z % RegionMapData.SizeZ; } }
        public static bool IsRegionView { get; set; }
    }
}

