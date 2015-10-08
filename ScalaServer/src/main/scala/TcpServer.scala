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

  def receive = {
    case Bound(local) =>
      log.debug("Bound to {}", local)

    case CommandFailed(cmd, reason) =>
      log.error("Command Failed: {}", reason)
      context.stop(self)

    case Connected(remote, local) =>
      log.debug("New connection from {}", remote)
      val connection = sender()
      context.actorOf(handler(connection))
  }
}
