using System;

namespace Sean.World
{
    // see http://flafla2.github.io/2014/08/09/perlinnoise.html

	internal class PerlinNoise
	{


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

        public static Array<int> GetIntMap(ArraySize size, int minX, int maxX, int minZ, int maxZ, int octaveCount)
        {
            var perlin = new PerlinNoise();
            var noise = new Array<int>(size);
            for (int z = minZ; z < maxZ; z += noise.Size.scale)
            {
                for (int x = minX; x < maxX; x += noise.Size.scale)
                {
                    double height = perlin.OctavePerlin (noise.Size, x,1,z, octaveCount, 1.0);
                    noise.Set(x,z, (int)(height*10));
                }
            }
            return noise;
        }

        public static Array<float> GetFloatMap(ArraySize size, int minX, int maxX, int minZ, int maxZ, int octaveCount)
        {
            var perlin = new PerlinNoise();
            var noise = new Array<float>(size);
            for (int z = minZ; z < maxZ; z += noise.Size.scale)
            {
                for (int x = minX; x < maxX; x += noise.Size.scale)
                {
                    double height = perlin.OctavePerlin(noise.Size, x, 1, z, octaveCount, 1.0);
                    noise.Set(x, z, (float)(height * 10));
                }
            }
            return noise;
        }

        public double OctavePerlin(ArraySize size, int x, int y, int z, int octaves, double persistence) {
            double xf = (double)x / size.maxX;
            double yf = 0.0;
            double zf = (double)z / size.maxZ;
            double total = 0;
            int frequency = 1;
            double amplitude = 1;
            double maxValue = 0;            // Used for normalizing result to 0.0 - 1.0
            for(int i=0;i<octaves;i++) {
                total += Perlin(size, xf, yf, zf) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total/maxValue;
        }

        private int p(int x, int y, int z) {
            return (int)(Misc.GetDeterministicInt (x, y, z, WorldSeed) % 256);
        }

        public double Perlin(ArraySize size, double x, double y, double z) {
            //if(repeat > 0) {                                    // If we have any repeat on, change the coordinates to their "local" repetitions
            //    x = x%repeat;
            //    y = y%repeat;
            //    z = z%repeat;
            //}

            int xi = (int)x;// & 255;                     // Calculate the "unit cube" that the point asked will be located in
            int yi = (int)y;// & 255;                     // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
            int zi = (int)z;// & 255;                     // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
            double xf = x-(int)x;                         // We also fade the location to smooth the result.
            double yf = y-(int)y;
            double zf = z-(int)z;

            int aaa, aba, aab, abb, baa, bba, bab, bbb;
            aaa = p(    xi ,    yi ,    zi );
            aba = p(    xi , (yi++),    zi );
            aab = p(    xi ,    yi , (zi++));
            abb = p(    xi , (yi++), (zi++));
            baa = p( (xi++),    yi ,    zi );
            bba = p( (xi++), (yi++),    zi );
            bab = p( (xi++),    yi , (zi++));
            bbb = p( (xi++), (yi++), (zi++));

            double u = xf;//fade(xf);
            double v = yf;//fade(yf);
            double w = zf;//fade(zf);

            double x1, x2, y1, y2;
            x1 = Lerp(    Grad (aaa, xf  , yf  , zf),   // The gradient function calculates the dot product between a pseudorandom
                Grad (baa, xf-1, yf  , zf),             // gradient vector and the vector from the input coordinate to the 8
                u);                                     // surrounding points in its unit cube.
            x2 = Lerp(    Grad (aba, xf  , yf-1, zf),   // This is all then lerped together as a sort of weighted average based on the faded (u,v,w)
                Grad (bba, xf-1, yf-1, zf),             // values we made earlier.
                u);
            y1 = Lerp(x1, x2, v);

            x1 = Lerp(    Grad (aab, xf  , yf  , zf-1),
                Grad (bab, xf-1, yf  , zf-1),
                u);
            x2 = Lerp(    Grad (abb, xf  , yf-1, zf-1),
                Grad (bbb, xf-1, yf-1, zf-1),
                u);
            y2 = Lerp (x1, x2, v);

            return (Lerp (y1, y2, w)+1)/2;               // For convenience we bind the result to 0 - 1 (theoretical min/max before is [-1, 1])
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

        private static float Lerp(float a, float b, float w)
        {
            return a + w * (b - a);
        }
        public static double Lerp(double a, double b, double w)
        {
            return a + w * (b - a);
        }

        private static int Lerp(int minY, int maxY, float t)
        {
            float u = 1 - t;
            return (int)(minY * u + maxY * t);
        }

        private static double Fade(double t)
        {
            // Fade function as defined by Ken Perlin.  This eases coordinate values
            // so that they will "ease" towards integral values.  This ends up smoothing
            // the final output.
            return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
        }

        public int WorldSeed { get; set; }
        //public int repeat;
	}
}