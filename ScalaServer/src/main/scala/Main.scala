package com.example.akkaTcpChat

import akka.actor.{Props, ActorSystem}
import scala.concurrent.duration._
import java.net.InetSocketAddress
//import com.example.akkaTcpChat.client.{InputUserMessage, UserInteract}

class Application extends Bootable 
{
  implicit val actorSystem = ActorSystem("tcpserver")
  implicit val executor = actorSystem.dispatcher

  lazy val addr = {
    new InetSocketAddress("localhost", 8842)
  }

  def startup() = 
  {
    // Register all needed actors
    //val server = actorSystem.actorOf(Server.props(addr), "server")
    val world = actorSystem.actorOf(World.props())

    val tcpServer = actorSystem.actorOf(Props(classOf[TcpServer], addr, classOf[SimpleEchoHandler]), "simple")

    // TODO - Move to own actor
    world ! "init"
    world ! "dump"
    //val d = world ! new Get(1,1)
    //Console.println("Data:"+d)
    world ! new Set(1,1,9,1)
    world ! "dump"
    
    //var perlin = new perlinNoise.PerlinNoise()
    ////var noise = perlin.GenerateWhiteNoise(10,10)
    //var noise = perlin.GetIntMap(80, 30, 0, 9, 3)
    //noise.Dump()
 
  }

  def shutdown() =
  {
    actorSystem.shutdown()
    actorSystem.awaitTermination(3.seconds)
  }
}

object Application 
{
  def main(args: Array[String])
  {
    val app = new Application()
    app.startup()
  }
}

trait Bootable
{
  def startup(): Unit
  def shutdown(): Unit

  sys.ShutdownHookThread(shutdown())
}

