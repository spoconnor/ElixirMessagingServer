// TcpServer.scala
package com.example.akkaTcpChat

import java.net.InetSocketAddress

import akka.actor._
import akka.io.Tcp._
import akka.io._

class TcpServer(bind: InetSocketAddress, handler: ActorRef => Props)
    extends Actor with ActorLogging {
  implicit val system = context.system
  IO(Tcp) ! Bind(self, bind)

  log.info("TcpServer: initializing")

  def receive = {
    case Bound(local) =>
      log.info("TcpServer: Bound to {}", local)

    case CommandFailed(cmd, reason) =>
      log.error("TcpServer: Command Failed: {}", reason)
      context.stop(self)

    case Connected(remote, local) =>
      log.info("TcpServer: New connection from {}", remote)
      val handler = context.actorOf(handler(sender(), remote))
      sender() ! Register(self)
  }
}
