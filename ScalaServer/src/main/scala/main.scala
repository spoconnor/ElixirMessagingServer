import akka.actor._
import akka.io.{ IO, Tcp }
import java.net.InetSocketAddress

object App extends App {
  final val VERSION = "1.0-SCALASERVICE"
  final val CODENAME = "Scala Service"

  implicit var actorSystem: ActorSystem = _

  override def main(args: Array[String]) 
  {
    Console.println("Hello World")
    var perlin = new perlinNoise.PerlinNoise()
    //var noise = perlin.GenerateWhiteNoise(10,10)
    var noise = perlin.GetIntMap(80, 30, 0, 9, 3)
    noise.Dump()

    Console.println("Loading new Actor System")
    val actorSystem = ActorSystem("NetworkServiceSystem")

    Console.println("Starting Network Service")


    readLine("Hit ENTER to exit...")
    actorSystem.shutdown()
  }
}
