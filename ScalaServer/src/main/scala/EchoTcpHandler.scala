package com.example.akkaTcpChat

// EchoTcpHandler.scala
import akka.actor._
import akka.io.Tcp._

class EchoTcpHandler(connection: ActorRef) extends Actor {
  connection ! Register(self)

  def receive = {
    case Received(data) => connection ! Write(data)
  }
}
