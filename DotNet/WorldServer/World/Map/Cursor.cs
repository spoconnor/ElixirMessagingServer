using System;
namespace Sean.World
{
    public static class Cursor
    {
        public static void MoveLeft() { Z--; }
        public static void MoveRight() { Z++; }
        public static void MoveDown() { X++; }
        public static void MoveUp() { X--; }

        public static int X { get; set; }
        public static int Z { get; set; }
    }
}

