import akka.actor.Actor
import akka.actor.Props
import akka.event.Logging

class Map extends Actor {
  var log = Logging(context.system, this)
  def recevie = {
    case "init" init()
    case "get" chunk ! GetChunk(1,1)
    case _ log.info("Map received unknown message")
  }

  def init = {
    log.info("init")
    //val chunk = system.actorOf(Props[MapChunk], name = "chunk1")
    val chunk = system.actorOf(Props(new MapChunk(1,1)), name = "chunk-1-1")
  }
}
