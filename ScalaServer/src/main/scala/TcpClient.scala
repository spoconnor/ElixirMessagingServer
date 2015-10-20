// TcpClient.scala
package com.example.akkaTcpChat

import akka.actor._
import akka.io._
import akka.io.Tcp._
import java.net.InetSocketAddress
import akka.util.ByteString
import scala.concurrent.duration._

class TcpClient(remote: InetSocketAddress) extends Actor with ActorLogging {
 
  import Tcp._
  import context.system
  import scala.concurrent.ExecutionContext.Implicits.global

  log.info("TcpClient: initializing connection to {}", remote)
  IO(Tcp) ! Connect(remote)
 
  def receive = {
    case "reconnect" =>
      IO(Tcp) ! Connect(remote)

    case CommandFailed(_: Connect) =>
      log.error("TcpClient: connect failed")
      val s = sender
      context.system.scheduler.scheduleOnce(5 seconds, self, "reconnect")
      //context stop self
 
    case c @ Connected(remote, local) =>
      log.info("TcpClient: Connected")
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
          //val s = sender
          //context.system.scheduler.scheduleOnce(5 seconds, self, "reconnect")
          // TODO - send message to parent context?
          context stop self
      }
  }
}
