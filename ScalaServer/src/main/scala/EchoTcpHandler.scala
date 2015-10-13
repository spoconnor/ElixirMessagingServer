package com.example.akkaTcpChat

// EchoTcpHandler.scala
import akka.actor._
import akka.io.Tcp._

class EchoTcpHandler(connection: ActorRef) extends Actor with ActorLogging {
  connection ! Register(self)

  log.info("EchoTcpHandler: initializing")

  def receive = {
    case Received(data) =>
      log.info("EchoTcpHandler: Received Data")
      connection ! Write(data)
  }
}
