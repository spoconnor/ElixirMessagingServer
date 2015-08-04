object PerlinNoise
{
    def GenerateWhiteNoise(width: Int, height: Int): Float[][] =
		{
      Float[][] noise = Misc.GetEmptyArray<Float>(width, height)

			for (Int i = 0; i < width; i++)
			{
				for (Int j = 0; j < height; j++)
				{
					noise[i][j] = (Float)Settings.Random.NextDouble()
				}
			}
			return noise
		}

		def Interpolate(x0: Float, x1: Float, alpha: Float): Float =
		{
			return x0 * (1 - alpha) + alpha * x1
		}

		def Interpolate(minY: Int, maxY: Int, t: Float): Int =
		{
			Float u = 1 - t
			return (Int)(minY * u + maxY * t)
		}

		def MapInts(minY: Int, maxY: Int, perlinNoise: Float[][]): Int[][] =
		{
			Int width = perlinNoise.Length
			Int height = perlinNoise[0].Length
      Int[][] heightMap = Misc.GetEmptyArray<Int>(width, height)

			for (Int i = 0; i < width; i++)
			{
				for (Int j = 0; j < height; j++)
				{
					heightMap[i][j] = Interpolate(minY, maxY, perlinNoise[i][j])
				}
			}
			return heightMap
		}

		def MapFloats(minY: Float, maxY: Float, perlinNoise: Float[][]): Float[][] =
		{
			Int width = perlinNoise.Length
			Int height = perlinNoise[0].Length
			Float[][] treeMap = Misc.GetEmptyArray<Float>(width, height)

			for (Int i = 0; i < width; i++)
			{
				for (Int j = 0; j < height; j++)
				{
					treeMap[i][j] = Interpolate(minY, maxY, perlinNoise[i][j])
				}
			}
			return treeMap
		}

		def GenerateSmoothNoise(baseNoise: Float[][], octave: Int): Float[][] =
		{
			Int width = baseNoise.Length
			Int height = baseNoise[0].Length

      Float[][] smoothNoise = Misc.GetEmptyArray<Float>(width, height)
			Int samplePeriod = 1 << octave // calculates 2 ^ k
			Float sampleFrequency = 1.0f / samplePeriod

			for (Int i = 0; i < width; i++)
			{
				//calculate the horizontal sampling indices
				Int iSample0 = (i / samplePeriod) * samplePeriod
				Int iSample1 = (iSample0 + samplePeriod) % width //wrap around
				Float horizontalBlend = (i - iSample0) * sampleFrequency

				for (Int j = 0; j < height; j++)
				{
					//calculate the vertical sampling indices
					Int jSample0 = (j / samplePeriod) * samplePeriod
					Int jSample1 = (jSample0 + samplePeriod) % height //wrap around
					Float verticalBlend = (j - jSample0) * sampleFrequency

					//blend the top two corners
					Float top = Interpolate(baseNoise[iSample0][jSample0], baseNoise[iSample1][jSample0], horizontalBlend)

					//blend the bottom two corners
					Float bottom = Interpolate(baseNoise[iSample0][jSample1], baseNoise[iSample1][jSample1], horizontalBlend)

					//final blend
					smoothNoise[i][j] = Interpolate(top, bottom, verticalBlend)
				}
			}
			return smoothNoise
		}

		def GeneratePerlinNoise(baseNoise: Float[][], octaveCount: Int): Float[][] =
		{
			Int width = baseNoise.Length
			Int height = baseNoise[0].Length

			var smoothNoise = new Float[octaveCount][][] //an array of 2D arrays containing

			const Float PERSISTANCE = 0.4f

			//generate smooth noise
			for (Int i = 0; i < octaveCount; i++)
			{
				smoothNoise[i] = GenerateSmoothNoise(baseNoise, i)
			}

      Float[][] perlinNoise = Misc.GetEmptyArray<Float>(width, height) //an array of floats initialised to 0
			Float amplitude = 1f
			Float totalAmplitude = 0.0f

			//blend noise together
			for (Int octave = octaveCount - 1; octave >= 0; octave--)
			{
				amplitude *= PERSISTANCE
				totalAmplitude += amplitude

				for (Int i = 0; i < width; i++)
				{
					for (Int j = 0; j < height; j++)
					{
						perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude
					}
				}
			}

			//normalisation
			for (Int i = 0; i < width; i++)
			{
				for (Int j = 0; j < height; j++)
				{
					perlinNoise[i][j] /= totalAmplitude
				}
			}
			return perlinNoise
		}

		def GetIntMap(width :Int, height: Int, minY: Int, maxY: Int, octaveCount: Int): Int[][] =
		{
      Float[][] baseNoise = GenerateWhiteNoise(width, height)
			Float[][] perlinNoise = GeneratePerlinNoise(baseNoise, octaveCount)
			return MapInts(minY, maxY, perlinNoise)
		}

    def GetFloatMap(width: Int, height: Int, minY: Float, maxY: Float, octaveCount: Int): Float[][] =
		{
      Float[][] baseNoise = GenerateWhiteNoise(width, height)
			Float[][] perlinNoise = GeneratePerlinNoise(baseNoise, octaveCount)
			return MapFloats(minY, maxY, perlinNoise)
		}
}
