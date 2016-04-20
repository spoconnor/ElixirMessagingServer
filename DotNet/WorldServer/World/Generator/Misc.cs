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

        private static byte[] GenerateDeterministicHash(int x, int y, int z, int worldSeed)
        {
            using (MD5 md5Hash = MD5.Create ()) 
            {
                string input = $"{x}-{y}-{z}-{worldSeed}";
                // Convert the input string to a byte array and compute the hash.
                return md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }
        private static byte[] GenerateDeterministicHash(int x, int z, int worldSeed)
        {
            using (MD5 md5Hash = MD5.Create ()) 
            {
                string input = $"{x}-{z}-{worldSeed}";
                // Convert the input string to a byte array and compute the hash.
                return md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }
        public static float GetDeterministicFloat (int x, int z, int worldSeed)
        {
            byte[] data = GenerateDeterministicHash(x,z,worldSeed);
            return System.BitConverter.ToSingle(data, 0);
        }
        public static uint GetDeterministicInt (int x, int y, int z, int worldSeed)
        {
            byte[] data = GenerateDeterministicHash(x,y,z,worldSeed);
            return System.BitConverter.ToUInt32(data, 0);
        }
        public static uint GetDeterministicInt (int x, int z, int worldSeed)
        {
            byte[] data = GenerateDeterministicHash(x,z,worldSeed);
            return System.BitConverter.ToUInt32(data, 0);
        }

    }
}

