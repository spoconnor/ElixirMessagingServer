import akka.actor._
import akka.io.{ IO, Tcp }
import java.net.InetSocketAddress

object App extends App {
  final val VERSION = "1.0-SCALASERVICE"
  final val CODENAME = "Scala Service"

  implicit var actorSystem: ActorSystem = _

  override def main() 
  {
    Console.println("Hello World")
    var perlin = new perlinNoise.PerlinNoise()
    //var noise = perlin.GenerateWhiteNoise(10,10)
    var noise = perlin.GetIntMap(80, 30, 0, 9, 3)
    noise.Dump()

    Console.println("Loading new Actor System")
    val actorSystem = ActorSystem("NetworkServiceSystem")

    Console.println("Starting Network Service")
    val endpoint = new InetSocketAddress("localhost", 9999)
    val handler = actorSystem.actorOf(Props[NetworkService], name = "network-handler")
    IO(Tcp) ! Tcp.Bind(handler, endpoint)

    readLine("Hit ENTER to exit...")
    actorSystem.shutdown()
  }
}
