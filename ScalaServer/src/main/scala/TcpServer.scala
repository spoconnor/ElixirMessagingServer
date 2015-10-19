// TcpServer.scala
package com.example.akkaTcpChat

import java.net.InetSocketAddress

import akka.actor._
import akka.io.Tcp._
import akka.io._

class TcpServer(bind: InetSocketAddress, handlerClass: Class[_]) extends Actor with ActorLogging {

  import Tcp._
  import context.system

  log.info("TcpServer: initializing")
  // there is not recovery for broken connections
  override val supervisorStrategy = SupervisorStrategy.stoppingStrategy

  // bind to the listen port; the port will automatically be closed once this actor dies
  override def preStart(): Unit = {
    IO(Tcp) ! Bind(self, bind)
  }

  // do not restart
  override def postRestart(thr: Throwable): Unit = context stop self

  def receive = {
    case Bound(localAddress) =>
      log.info("listening on port {}", localAddress.getPort)

    case CommandFailed(Bind(_, local, _, _, _)) =>
      log.warning(s"cannot bind to [$local]")
      context stop self

    //#echo-manager
    case Connected(remote, local) =>
      log.info("received connection from {}", remote)
      val handler = context.actorOf(Props(handlerClass, sender(), remote))
      sender() ! Register(handler, keepOpenOnPeerClosed = true)
    //#echo-manager
  }

}
