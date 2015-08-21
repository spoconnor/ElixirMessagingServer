import akka.actor._
import akka.util.{ ByteString, ByteStringBuilder }
import java.net.InetSocketAddress
//import akka.actor.Actor
//import akka.actor.Props
//import akka.event.Logging
 
//class MyActor extends Actor {
//  val log = Logging(context.system, this)
//  def receive = {
//    case "test" ⇒ log.info("received test")
//    case _      ⇒ log.info("received unknown message")
//  }
//

class NetworkService(port: Int) extends Actor {
  import IO._

  val state = IterateeRef.Map.async[IO.Handle]()(context.dispatcher)

  override def preStart {
    IOManager(context.system) listen new InetSocketAddress(port)
  }

  def receive = {

    case NewClient(server) =>
      val socket = server.accept()
      state(socket) flatMap (_ => NetworkService.printMessage)

    case Read(socket, bytes) =>
      state(socket)(Chunk(bytes))

    case Closed(socket, cause) =>
      state(socket)(EOF(None))
      state -= socket
  }

}

object NetworkService {
  import IO._
  def ascii(bytes: ByteString): String = bytes.decodeString("US-ASCII").trim

  def printMessage: IO.Iteratee[Unit] =
    repeat {
      for {
        string <- readMessage
 } yield {
    println(string)
    }
 }
def readMessage: IO.Iteratee[String] =
 for {
   lengthBytes <- take(4)
   len = ascii(lengthBytes).toInt
   bytes <- take(len)
 } yield {
   ascii(bytes)
 }
}
