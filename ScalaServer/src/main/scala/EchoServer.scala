package com.example.akkaTcpChat

import java.net.InetSocketAddress

import scala.concurrent.duration.DurationInt

import com.typesafe.config.ConfigFactory

import akka.actor.{ Actor, ActorDSL, ActorLogging, ActorRef, ActorSystem, Props, SupervisorStrategy }
import akka.actor.ActorDSL.inbox
import akka.io.{ IO, Tcp }
import akka.util.ByteString

//object EchoServer extends App {
//
//  val config = ConfigFactory.parseString("akka.loglevel = DEBUG")
//  implicit val system = ActorSystem("EchoServer", config)
//
//  // make sure to stop the system so that the application stops
//  try run()
//  finally system.shutdown()
//
//  def run(): Unit = {
//    import ActorDSL._
//
//    // create two EchoManager and stop the application once one dies
//    val watcher = inbox()
//    watcher.watch(system.actorOf(Props(classOf[EchoManager], classOf[EchoHandler]), "echo"))
//    watcher.watch(system.actorOf(Props(classOf[EchoManager], classOf[SimpleEchoHandler]), "simple"))
//    watcher.receive(10.minutes)
//  }
//
//}

class EchoManager(handlerClass: Class[_]) extends Actor with ActorLogging {

  import Tcp._
  import context.system

  // there is not recovery for broken connections
  override val supervisorStrategy = SupervisorStrategy.stoppingStrategy

  // bind to the listen port; the port will automatically be closed once this actor dies
  override def preStart(): Unit = {
    IO(Tcp) ! Bind(self, new InetSocketAddress("localhost", 0))
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

