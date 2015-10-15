package com.example.akkaTcpChat

// EchoTcpHandler.scala

import java.net.InetSocketAddress
import akka.actor._
import akka.io.{ IO, Tcp }

class EchoTcpHandler(connection: ActorRef, remote: InetSocketAddress) extends Actor with ActorLogging {


  log.info("EchoTcpHandler: initializing")

  def receive = {
    case Received(data) =>
      log.info("EchoTcpHandler: Received Data")
      connection ! Write(data)
  }
}
