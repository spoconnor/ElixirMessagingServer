object perlinNoise 
{
  class PerlinNoise() 
  {   
    def GenerateWhiteNoise(width: Int, height: Int): misc.FloatArray =
		{
      var noise = new misc.FloatArray(width, height)
      val rand = new scala.util.Random
      noise.Map((i:Float) => rand.nextFloat())
      noise
		}

		def Interpolate(x0: Float, x1: Float, alpha: Float): Float =
		{
			x0 * (1 - alpha) + alpha * x1
		}

		def Interpolate(minY: Int, maxY: Int, t: Float): Int =
		{
			var u = 1 - t
			(minY * u + maxY * t).toInt
		}

		def MapInts(minY: Int, maxY: Int, perlinNoise: misc.FloatArray): misc.IntArray =
		{
      var heightMap = new misc.IntArray(perlinNoise.Width, perlinNoise.Height)
      for (i <- 0 to perlinNoise.Width-1)
      {
        for (j <- 0 to perlinNoise.Height-1)
        {
          var f = perlinNoise.Get(i,j)
          heightMap.Set(i, j, this.Interpolate(minY, maxY, f))
        }
      }
      heightMap 
		}

		def MapFloats(minY: Float, maxY: Float, perlinNoise: misc.FloatArray): misc.FloatArray =
		{
			var treeMap = new misc.FloatArray(perlinNoise.Width, perlinNoise.Height)
      for (i <- 0 to perlinNoise.Width-1)
      {
        for (j <- 0 to perlinNoise.Height-1)
        {
          var f = perlinNoise.Get(i,j)
          treeMap.Set(i, j, this.Interpolate(minY, maxY, f))
        }
      }
      treeMap
		}

		def GenerateSmoothNoise(baseNoise: misc.FloatArray, octave: Int): misc.FloatArray =
		{
      var smoothNoise = new misc.FloatArray(baseNoise.Width, baseNoise.Height)
			var samplePeriod = 1 << octave // calculates 2 ^ k
			var sampleFrequency = 1.0f / samplePeriod

			for (i <- 0 to baseNoise.Width-1)
			{
				//calculate the horizontal sampling indices
				var iSample0 = (i / samplePeriod) * samplePeriod
				var iSample1 = (iSample0 + samplePeriod) % baseNoise.Width //wrap around
				var horizontalBlend = (i - iSample0) * sampleFrequency

				for (j <- 0 to baseNoise.Height-1)
				{
					//calculate the vertical sampling indices
					var jSample0 = (j / samplePeriod) * samplePeriod
					var jSample1 = (jSample0 + samplePeriod) % baseNoise.Height //wrap around
					var verticalBlend = (j - jSample0) * sampleFrequency

					//blend the top two corners
					var top = Interpolate(baseNoise.Get(iSample0,jSample0), baseNoise.Get(iSample1,jSample0), horizontalBlend)

					//blend the bottom two corners
					var bottom = Interpolate(baseNoise.Get(iSample0,jSample1), baseNoise.Get(iSample1,jSample1), horizontalBlend)

					//final blend
					smoothNoise.Set(i,j, Interpolate(top, bottom, verticalBlend))
				}
			}
			smoothNoise
		}

		def GeneratePerlinNoise(baseNoise: misc.FloatArray, octaveCount: Int): misc.FloatArray =
		{
			var smoothNoise = Array.ofDim[misc.FloatArray](octaveCount) //an array of 2D arrays containing
			var PERSISTANCE = 0.4f

			//generate smooth noise
			for (i <- 0 to octaveCount-1)
			{
				smoothNoise(i) = GenerateSmoothNoise(baseNoise, i)
			}

      var perlinNoise = new misc.FloatArray(baseNoise.Width, baseNoise.Height) //an array of floats initialised to 0
			var amplitude = 1f
			var totalAmplitude = 0.0f

			//blend noise together
			for (octave <- octaveCount - 1 to 0 by -1)
			{
				amplitude *= PERSISTANCE
				totalAmplitude += amplitude

				for (i <- 0 to baseNoise.Width-1)
				{
					for (j <- 0 to baseNoise.Height-1)
					{
						perlinNoise.Set(i,j, perlinNoise.Get(i,j) + smoothNoise(octave).Get(i,j) * amplitude)
					}
				}
			}

			//normalisation
      perlinNoise.Map((i:Float) => i / totalAmplitude)
      perlinNoise
    }

		def GetIntMap(width :Int, height: Int, minY: Int, maxY: Int, octaveCount: Int): misc.IntArray =
		{
      var baseNoise = GenerateWhiteNoise(width, height)
			var perlinNoise = GeneratePerlinNoise(baseNoise, octaveCount)
      MapInts(minY, maxY, perlinNoise)
		}

    def GetFloatMap(width: Int, height: Int, minY: Float, maxY: Float, octaveCount: Int): misc.FloatArray =
		{
      var baseNoise = GenerateWhiteNoise(width, height)
			var perlinNoise = GeneratePerlinNoise(baseNoise, octaveCount)
			MapFloats(minY, maxY, perlinNoise)
		}
  }
}
