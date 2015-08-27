import akka.actor.{ Actor, ActorRef, Props }
import akka.io.{ IO, Tcp }
import akka.util.{ ByteString, ByteStringBuilder }
import java.net.InetSocketAddress

object NetworkService {
  def props(endpoint: InetSocketAddress): Props = 
    Props(new NetworkService(endpoint))
}

class NetworkService(endpoint: InetSocketAddress) extends Actor with ActorLogging {
  IO(Tcp) ! Tcp.Bind(self, endpoint)
}
