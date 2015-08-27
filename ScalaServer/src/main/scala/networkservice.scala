import akka.actor.{ Actor, ActorRef, Props }
import akka.io.{ IO, Tcp }
import akka.util.{ ByteString, ByteStringBuilder }
import java.net.InetSocketAddress

object NetworkService {
  def props(endpoint: InetSocketAddress): Props = 
    Props(new NetworkService(endpoint))
}

class NetworkService(endpoint: InetSocketAddress) extends Actor with akka.actor.ActorLogging {
  override def receive: Receive = {
    case Tcp.Connected(remote, _) =>
      log.debug("Remote address {} connected", remote)
      sender ! Tcp.Register(context.actorOf(EchoConnectionHandler.props(remote, sender)))
  }

}

object EchoConnectionHandler {
  def props(remote: InetSocketAddress, connection: ActorRef): Props =
    Props(new EchoConnectionHandler(remote, connection))
}

class EchoConnectionHandler(remote: InetSocketAddress, connection: ActorRef) extends Actor with ActorLogging {

  // We need to know when the connection dies without sending a `Tcp.ConnectionClosed`
  context.watch(connection)

  def receive: Receive = {
    case Tcp.Received(data) =>
      val text = data.utf8String.trim
      log.debug("Received '{}' from remote address {}", text, remote)
      text match {
        case "close" => context.stop(self)
        case _       => sender ! Tcp.Write(data)
      }
    case _: Tcp.ConnectionClosed =>
      log.debug("Stopping, because connection for remote address {} closed", remote)
      context.stop(self)
    case Terminated(`connection`) =>
      log.debug("Stopping, because connection for remote address {} died", remote)
      context.stop(self)
  }
}
