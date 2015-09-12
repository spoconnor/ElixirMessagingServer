import akka.actor.Actor
import akka.actor.Props
import akka.event.Logging

class Map extends Actor {
  import context.system
  var log = Logging(context.system, this)
  //val chunk = system.actorOf(Props[MapChunk], name = "chunk1")
  val chunk = system.actorOf(Props(new MapChunk(1,1)), name = "chunk-1-1")

  def receive = {
    case "init" => init
    case "get" => chunk ! new GetChunk(1,1)
    case _ => log.info("Map received unknown message")
  }

  def init = {
    log.info("init")
  }
}
