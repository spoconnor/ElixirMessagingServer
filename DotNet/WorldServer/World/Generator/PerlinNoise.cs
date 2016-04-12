﻿using System;

namespace Sean.World
{
	internal class PerlinNoise
	{
        private static void GenerateWhiteNoise(Array<float> array, int worldSeed)
		{
            for (int z = array.Size.minZ; z < array.Size.maxZ; z += array.Size.scale) {
                for (int x = array.Size.minX; x < array.Size.maxX; x += array.Size.scale) {
                    array.Set (x, z, Misc.GetDeterministicInt (x, z, worldSeed));
                }
            }		
		}

        private static float Lerp(float a, float b, float w)
		{
            return a + w * (b - a);
		}
        public static double Lerp(double a, double b, double w) {
            return a + w * (b - a);
        }

		private static int Lerp(int minY, int maxY, float t)
        {
			float u = 1 - t;
			return (int)(minY * u + maxY * t);
		}

        public static double Fade(double t) {
            // Fade function as defined by Ken Perlin.  This eases coordinate values
            // so that they will "ease" towards integral values.  This ends up smoothing
            // the final output.
            return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
        }

        private static Array<int> MapInts(int minY, int maxY, Array<float> perlinNoise)
		{
            var heightMap = new Array<int>(perlinNoise.Size);
            for (int z = heightMap.Size.minZ; z < heightMap.Size.maxZ; z += heightMap.Size.scale) {
                for (int x = heightMap.Size.minX; x < heightMap.Size.maxX; x += heightMap.Size.scale) {
                    heightMap.Set (x, z, Lerp(minY, maxY, perlinNoise.Get(x,z)));
                }
            }
			return heightMap;
		}

        private static Array<float> MapFloats(int minY, int maxY, Array<float> perlinNoise)
        {
            var heightMap = new Array<float>(perlinNoise.Size);
            for (int z = heightMap.Size.minZ; z < heightMap.Size.maxZ; z += heightMap.Size.scale) {
                for (int x = heightMap.Size.minX; x < heightMap.Size.maxX; x += heightMap.Size.scale) {
                    heightMap.Set (x, z, Lerp(minY, maxY, perlinNoise.Get(x,z)));
                }
            }
            return heightMap;
        }

        private static Array<float> GenerateSmoothNoise(Array<float> baseNoise, int octave)
		{
            var smoothNoise = new Array<float> (baseNoise.Size);
            int samplePeriod = 1 << (octave + baseNoise.Size.scale); // calculates 2 ^ k
			float sampleFrequency = 1.0f / samplePeriod;

            for (int i = baseNoise.Size.minZ; i < baseNoise.Size.maxZ; i += baseNoise.Size.scale) 
			{
				//calculate the horizontal sampling indices
				int iSample0 = (i / samplePeriod) * samplePeriod;
                int iSample1 = (iSample0 + samplePeriod);
                if (iSample1 > baseNoise.Size.maxZ) iSample1 = iSample1 - baseNoise.Size.zWidth; //wrap around
				float horizontalBlend = (i - iSample0) * sampleFrequency;

                for (int j = baseNoise.Size.minX; j < baseNoise.Size.maxX; j += baseNoise.Size.scale) 
				{
					//calculate the vertical sampling indices
					int jSample0 = (j / samplePeriod) * samplePeriod;
                    int jSample1 = (jSample0 + samplePeriod);
                    if (jSample1 > baseNoise.Size.maxX) jSample1 = jSample1 - baseNoise.Size.xHeight; //wrap around
					float verticalBlend = (j - jSample0) * sampleFrequency;

					//blend the top two corners
                    float top = Lerp(baseNoise.Get(jSample0,iSample0), baseNoise.Get(jSample0,iSample1), horizontalBlend);

					//blend the bottom two corners
                    float bottom = Lerp(baseNoise.Get(jSample1,iSample0), baseNoise.Get(jSample1,iSample1), horizontalBlend);

					//final blend
                    smoothNoise.Set(j,i,Lerp(top, bottom, verticalBlend));
				}
			}
			return smoothNoise;
		}

        private static Array<float> GeneratePerlinNoise(Array<float> baseNoise, int octaveCount)
		{
            var smoothNoise = new Array<float>[octaveCount]; // an array of 2D arrays
			const float PERSISTANCE = 0.4f;

			//generate smooth noise
			for (int i = 0; i < octaveCount; i++)
			{
				smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
			}

            var perlinNoise = new Array<float>(baseNoise.Size); //an array of floats initialised to 0

			float amplitude = 1f;
			float totalAmplitude = 0.0f;

			//blend noise together
			for (int octave = octaveCount - 1; octave >= 0; octave--)
			{
				amplitude *= PERSISTANCE;
				totalAmplitude += amplitude;

                for (int i = baseNoise.Size.minZ; i < baseNoise.Size.maxZ; i += baseNoise.Size.scale) 
                {
                    for (int j = baseNoise.Size.minX; j < baseNoise.Size.maxX; j += baseNoise.Size.scale) 
                    {
                        perlinNoise.Set(j,i, perlinNoise.Get(j,i) + smoothNoise[octave].Get(j,i) * amplitude);
					}
				}
			}

			//normalisation
            for (int i = baseNoise.Size.minZ; i < baseNoise.Size.maxZ; i += baseNoise.Size.scale) 
            {
                for (int j = baseNoise.Size.minX; j < baseNoise.Size.maxX; j += baseNoise.Size.scale) 
                {
                    perlinNoise.Set(j,i, perlinNoise.Get(j,i) / totalAmplitude);
                }
			}
			return perlinNoise;
		}

        /*
		public static Array<int> GetIntMap(ArraySize size, int octaveCount)
		{
            var baseNoise = new Array<float>(size);
            GenerateWhiteNoise(baseNoise, WorldSeed);
            var perlinNoise = new Array<float>(size);
			GeneratePerlinNoise(baseNoise, perlinNoise, octaveCount);
			return MapInts(minY, maxY, perlinNoise);
		}

        public static Array<float> GetFloatMap(ArraySize size, int octaveCount)
		{
            var baseNoise = new Array<float>(size);
            GenerateWhiteNoise(baseNoise, WorldSeed);
            var perlinNoise = new Array<float>(size);
            GeneratePerlinNoise(baseNoise, perlinNoise, octaveCount);
            return MapFloats(minY, maxY, perlinNoise);
		}
        */

        public static int[][] GetIntMap(int SizeX, int SizeZ, int MinY, int MaxY, int Octaves)
        {
            return new int[SizeX][];
        }
        public static float[][] GetFloatMap(int SizeInBlocksX, int SizeInBlocksZ, int a, int MAX_SURFACE_HEIGHT, int b)
        {
            return new float[SizeInBlocksX][];
        }

        public static Array<int> GetIntMap(ArraySize size, int octaveCount)
        {
            var perlin = new PerlinNoise();
            var noise = new Array<int>(size);
            for (int z = noise.Size.minZ; z < noise.Size.maxZ; z += noise.Size.scale)
            {
                for (int x = noise.Size.minX; x < noise.Size.maxX; x += noise.Size.scale)
                {
                    float iNorm = noise.Size.NormalizeZ(z);
                    float jNorm = noise.Size.NormalizeX(x);
                    double height = perlin.OctavePerlin (iNorm, jNorm, 0.0, octaveCount, 1.0);
                    noise.Set(x,z, (int)(height*10));
                }
            }
            return noise;
        }


        //---------------
        // see http://flafla2.github.io/2014/08/09/perlinnoise.html

        public int repeat;

        public PerlinNoise(int repeat = -1) {
            this.repeat = repeat;
        }

        public double OctavePerlin(double x, double y, double z, int octaves, double persistence) {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;            // Used for normalizing result to 0.0 - 1.0
            for(int i=0;i<octaves;i++) {
                total += Perlin(x * frequency, y * frequency, z * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total/maxValue;
        }

        /*
        private static readonly int[] permutation = { 151,160,137,91,90,15,                 // Hash lookup table as defined by Ken Perlin.  This is a randomly
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,    // arranged array of all numbers from 0-255 inclusive.
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private static readonly int[] p;                                                    // Doubled permutation to avoid overflow

        static PerlinNoise() {
            p = new int[512];
            for(int x=0;x<512;x++) {
                p[x] = permutation[x%256];
            }
        }
        */

        private int p(int x, int y, int z) {
            return Misc.GetDeterministicInt (x, y, z, WorldSeed) % 256;
        }

        public double Perlin(double x, double y, double z) {
            if(repeat > 0) {                                    // If we have any repeat on, change the coordinates to their "local" repetitions
                x = x%repeat;
                y = y%repeat;
                z = z%repeat;
            }

            int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
            int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
            int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
            double xf = x-(int)x;                               // We also fade the location to smooth the result.
            double yf = y-(int)y;
            double zf = z-(int)z;
            double u = Fade(xf);
            double v = Fade(yf);
            double w = Fade(zf);

            int aaa, aba, aab, abb, baa, bba, bab, bbb;
            //aaa = p[p[p[    xi ]+    yi ]+    zi ];
            //aba = p[p[p[    xi ]+Inc(yi)]+    zi ];
            //aab = p[p[p[    xi ]+    yi ]+Inc(zi)];
            //abb = p[p[p[    xi ]+Inc(yi)]+Inc(zi)];
            //baa = p[p[p[Inc(xi)]+    yi ]+    zi ];
            //bba = p[p[p[Inc(xi)]+Inc(yi)]+    zi ];
            //bab = p[p[p[Inc(xi)]+    yi ]+Inc(zi)];
            //bbb = p[p[p[Inc(xi)]+Inc(yi)]+Inc(zi)];

            aaa = p(    xi,     yi,      zi );
            aba = p(    xi, Inc(yi),     zi );
            aab = p(    xi,     yi,  Inc(zi));
            abb = p(    xi, Inc(yi), Inc(zi));
            baa = p(Inc(xi),    yi,      zi );
            bba = p(Inc(xi),Inc(yi),     zi );
            bab = p(Inc(xi),    yi,  Inc(zi));
            bbb = p(Inc(xi),Inc(yi), Inc(zi));

            double x1, x2, y1, y2;
            x1 = Lerp(  Grad (aaa, xf  , yf  , zf),     // The gradient function calculates the dot product between a pseudorandom
                Grad (baa, xf-1, yf  , zf),             // gradient vector and the vector from the input coordinate to the 8
                u);                                     // surrounding points in its unit cube.
            x2 = Lerp(  Grad (aba, xf  , yf-1, zf),     // This is all then lerped together as a sort of weighted average based on the faded (u,v,w)
                Grad (bba, xf-1, yf-1, zf),             // values we made earlier.
                u);
            y1 = Lerp(x1, x2, v);

            x1 = Lerp(  Grad (aab, xf  , yf  , zf-1),
                Grad (bab, xf-1, yf  , zf-1),
                u);
            x2 = Lerp(  Grad (abb, xf  , yf-1, zf-1),
                Grad (bbb, xf-1, yf-1, zf-1),
                u);
            y2 = Lerp (x1, x2, v);

            return (Lerp (y1, y2, w)+1)/2;              // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
        }

        public int Inc(int num) {
            num++;
            if (repeat > 0) num %= repeat;

            return num;
        }

        public static double Grad(int hash, double x, double y, double z) {
            switch(hash & 0xF)
            {
            case 0x0: return  x + y;
            case 0x1: return -x + y;
            case 0x2: return  x - y;
            case 0x3: return -x - y;
            case 0x4: return  x + z;
            case 0x5: return -x + z;
            case 0x6: return  x - z;
            case 0x7: return -x - z;
            case 0x8: return  y + z;
            case 0x9: return -y + z;
            case 0xA: return  y - z;
            case 0xB: return -y - z;
            case 0xC: return  y + x;
            case 0xD: return -y + z;
            case 0xE: return  y - x;
            case 0xF: return -y - z;
            default: return 0; // never happens
            }
        }





        //---------------



        public int WorldSeed { get; set; }
	}
}