

/**
 * @author sean
 */
object main extends App {
  Console.println("Hello World")
  var perlin = new perlinNoise.PerlinNoise()
  //var noise = perlin.GenerateWhiteNoise(10,10)
  var noise = perlin.GetIntMap(80, 30, 0, 9, 3)
  noise.Dump()
}