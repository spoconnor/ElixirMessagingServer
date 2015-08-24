import akka.actor._
import akka.util.{ ByteString, ByteStringBuilder }
import java.net.InetSocketAddress
import akka.event.Logging
 
class NetworkService(port: Int) extends Actor {
  import IO._

  val state = IterateeRef.Map.async[IO.Handle]()(context.dispatcher)

  override def preStart {
    IOManager(context.system) listen new InetSocketAddress(port)
  }

  def receive = {
    case NewClient(server) =>
      log.info("NewClient")
      val socket = server.accept()
      state(socket) flatMap (_ => NetworkService.printMessage)

    case Read(socket, bytes) =>
      log.info("Read")
      state(socket)(Chunk(bytes))

    case Closed(socket, cause) =>
      log.info("Close")
      state(socket)(EOF(None))
      state -= socket
  }
}

object NetworkService {
  import IO._
  def ascii(bytes: ByteString): String = bytes.decodeString("US-ASCII").trim

  def printMessage: IO.Iteratee[Unit] = {
    repeat {
      for {
        string <- readMessage
      } yield {
        println(string)
      }
    }
  }
  def readMessage: IO.Iteratee[String] = {
    for {
      lengthBytes <- take(4)
      len = ascii(lengthBytes).toInt
      bytes <- take(len)
    } yield {
      ascii(bytes)
    }
  }
}
