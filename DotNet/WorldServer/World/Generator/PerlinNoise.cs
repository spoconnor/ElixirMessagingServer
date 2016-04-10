namespace Sean.World
{
	internal class PerlinNoise
	{
        private static void GenerateWhiteNoise(Array<float> array, int worldSeed)
		{
            for (int z = array.Size.minZ; z < array.Size.maxZ; z += array.Size.scale) {
                for (int x = array.Size.minX; x < array.Size.maxX; x += array.Size.scale) {
                    array.Set (x, z, Misc.GetDeterministicHash (x, z, worldSeed));
                }
            }		
		}

		private static float Interpolate(float x0, float x1, float alpha)
		{
			return x0 * (1 - alpha) + alpha * x1;
		}

		private static int Interpolate(int minY, int maxY, float t)
        {
			float u = 1 - t;
			return (int)(minY * u + maxY * t);
		}

        private static Array<int> MapInts(int minY, int maxY, Array<float> perlinNoise)
		{
            var heightMap = new Array<int>(perlinNoise.Size);
            for (int z = heightMap.Size.minZ; z < heightMap.Size.maxZ; z += heightMap.Size.scale) {
                for (int x = heightMap.Size.minX; x < heightMap.Size.maxX; x += heightMap.Size.scale) {
                    heightMap.Set (x, z, Interpolate(minY, maxY, perlinNoise.Get(x,z)));
                }
            }
			return heightMap;
		}

        private static Array<float> MapFloats(int minY, int maxY, Array<float> perlinNoise)
        {
            var heightMap = new Array<float>(perlinNoise.Size);
            for (int z = heightMap.Size.minZ; z < heightMap.Size.maxZ; z += heightMap.Size.scale) {
                for (int x = heightMap.Size.minX; x < heightMap.Size.maxX; x += heightMap.Size.scale) {
                    heightMap.Set (x, z, Interpolate(minY, maxY, perlinNoise.Get(x,z)));
                }
            }
            return heightMap;
        }

        private static Array<float> GenerateSmoothNoise(Array<float> baseNoise, int octave)
		{
			int width = baseNoise.Length;
			int height = baseNoise[0].Length;

            var smoothNoise = Misc.GetEmptyArray<float>(width, height);
			int samplePeriod = 1 << octave; // calculates 2 ^ k
			float sampleFrequency = 1.0f / samplePeriod;

			for (int i = 0; i < width; i++)
			{
				//calculate the horizontal sampling indices
				int iSample0 = (i / samplePeriod) * samplePeriod;
				int iSample1 = (iSample0 + samplePeriod) % width; //wrap around
				float horizontalBlend = (i - iSample0) * sampleFrequency;

				for (int j = 0; j < height; j++)
				{
					//calculate the vertical sampling indices
					int jSample0 = (j / samplePeriod) * samplePeriod;
					int jSample1 = (jSample0 + samplePeriod) % height; //wrap around
					float verticalBlend = (j - jSample0) * sampleFrequency;

					//blend the top two corners
					float top = Interpolate(baseNoise[iSample0][jSample0], baseNoise[iSample1][jSample0], horizontalBlend);

					//blend the bottom two corners
					float bottom = Interpolate(baseNoise[iSample0][jSample1], baseNoise[iSample1][jSample1], horizontalBlend);

					//final blend
                    smoothNoise.Set(i,j,Interpolate(top, bottom, verticalBlend));
				}
			}
			return smoothNoise;
		}

        private static Array<float> GeneratePerlinNoise(Array<float> baseNoise, int octaveCount)
		{
			int width = baseNoise.Length;
			int height = baseNoise[0].Length;

			var smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

			const float PERSISTANCE = 0.4f;

			//generate smooth noise
			for (int i = 0; i < octaveCount; i++)
			{
				smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
			}

            Array<float> perlinNoise = Misc.GetEmptyArray<float>(width, height); //an array of floats initialised to 0

			float amplitude = 1f;
			float totalAmplitude = 0.0f;

			//blend noise together
			for (int octave = octaveCount - 1; octave >= 0; octave--)
			{
				amplitude *= PERSISTANCE;
				totalAmplitude += amplitude;

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
					}
				}
			}

			//normalisation
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					perlinNoise[i][j] /= totalAmplitude;
				}
			}
			return perlinNoise;
		}

		public static Array<int> GetIntMap(int width, int height, int minY, int maxY, int octaveCount)
		{
            var baseNoise = new Array<float>();
            GenerateWhiteNoise(baseNoise, WorldSeed);
            var perlinNoise = new Array<float>();
			GeneratePerlinNoise(baseNoise, perlinNoise, octaveCount);
			return MapInts(minY, maxY, perlinNoise);
		}

        public static float[][] GetFloatMap(int width, int height, float minY, float maxY, int octaveCount)
		{
            var baseNoise = new Array<float>();
            GenerateWhiteNoise(baseNoise, WorldSeed);
            var perlinNoise = new Array<float>();
            GeneratePerlinNoise(baseNoise, perlinNoise, octaveCount);
            return MapFloats(minY, maxY, perlinNoise);
		}

        public int WorldSeed { get; set; }
	}
}