import akka.actor.{ Actor, ActorRef, Props }
import akka.io.{ IO, Tcp }
import akka.util.{ ByteString, ByteStringBuilder }
import java.net.InetSocketAddress

class NetworkService extends Actor {
  import Tcp._
  import context.system

  IO(Tcp) ! Bind(self, new InetSocketAddress("localhost", 9999))

  def receive = {
    case b @ Bound(localAddress) =>
      // TODO

    case CommandFailed(_: Bind) => context stop self

    case c @ Connected(remote, local) =>
      val handler = context.actorOf(Props[ServiceHandler])
      val connection = sender()
      connection ! Register(handler)
  }
}

class ServiceHandler extends Actor {
  import Tcp._
  def receive = {
    case Received(data) => 
      val text = data.utf8String.trim
      Console.println("Received message")
      Console.println(text)
      sender() ! Write(data)
    case PeerClosed     => 
      context stop self
  }
}
