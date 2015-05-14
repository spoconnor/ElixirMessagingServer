using System;

namespace Sean.World
{
    public static class Misc
    {
        public static T[][] GetEmptyArray<T>(int width, int height)
        {
            var image = new T[width][];

            for (int i = 0; i < width; i++)
            {
                image[i] = new T[height];
            }
            return image;
        }
    }
}

