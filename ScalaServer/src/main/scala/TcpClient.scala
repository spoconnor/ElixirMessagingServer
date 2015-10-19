// TcpClient.scala
package com.example.akkaTcpChat

import akka.actor._
import akka.io._
import java.net.InetSocketAddress
import akka.util.ByteString

class TcpClient(remote: InetSocketAddress) extends Actor with ActorLogging {
 
  import Tcp._
  import context.system
 
  log.info("TcpClient: initializing connection to {}", remote)
  IO(Tcp) ! Connect(remote)
 
  def receive = {
    case CommandFailed(_: Connect) =>
      log.error("TcpClient: connect failed")
      context stop self
 
    case c @ Connected(remote, local) =>
      val connection = sender()
      connection ! Register(self)
      context become {
        case data: ByteString =>
          connection ! Write(data)
        case CommandFailed(w: Write) =>
          // O/S buffer was full
          log.info("TcpClient: Write failed")
        case Received(data) =>
          log.info("TcpClient: Received data")
        case "close" =>
          connection ! Close
        case _: ConnectionClosed =>
          log.info("TcpClient: connection closed")
          context stop self
      }
  }
}
