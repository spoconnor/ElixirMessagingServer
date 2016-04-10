using System;
using System.Security.Cryptography;
using System.Text;

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

        public float GetDeterministicHash (int x, int z, int worldSeed)
        {
            using (MD5 md5Hash = MD5.Create ()) 
            {
                string input = $"{x}-{z}-{worldSeed}";
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return System.BitConverter.ToSingle(data, 0);
            }
        }

    }
}

