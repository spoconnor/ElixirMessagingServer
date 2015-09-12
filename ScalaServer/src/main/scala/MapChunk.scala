import akka.actor.Actor
import akka.actor.Props
import akka.event.Logging

case class GetChunk(xc: Int, yc: Int)

class MapChunk(xc: Int, yc: Int) extends Actor {
  import context.system
  val x: Int = xc
  val y: Int = yc
  var log = Logging(context.system, this)

  def receive = {
    case "init" => log.info("init")
    case GetChunk(x,y) => getChunk(x,y)
    case _ => log.info("MapChunk received unknown message")
  }
  
  override def preStart() = {
    // Initialization code
  }

  def getChunk(x: Int, y: Int) {
    log.info("Get " + x + "," + y)
  }

  override def toString(): String = "Chunk " + x + "," + y;
}
