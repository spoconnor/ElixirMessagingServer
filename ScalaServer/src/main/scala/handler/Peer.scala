package com.example.akkaTcpChat.handler

import akka.actor.{Props, ActorRef, Actor}
import akka.io.Tcp.{Write, PeerClosed, Received}
import akka.util.ByteString
import akka.event.Logging
import com.example.akkaTcpChat.Common
import scala.util.{Failure, Success}

object Peer {

  case class PeerMessage(name: String, msg: String)

  def props(clientId: String, connection: ActorRef): Props = {
    Props(new Peer(clientId, connection))
  }

}

class Peer(clientId: String,
           connection: ActorRef,
           var clientName: String = "Unknown") extends Actor {

  import context.system
  var log = Logging(context.system, this)

  def receive = {
    case Received(data) =>
      log.debug("Received: " + data)
      handleReceivedData(data)
    case Peer.PeerMessage(name, msg) =>
      log.debug("PeerMessage: " + name + ":" + msg)
      val req = new Common.Request(Common.OTHER_CLIENT_MESSAGE)
      req("name") = name
      req("msg") = msg
      req.serializeAsByteString match {
        case Success(b) =>
          connection ! Write(b)
        case Failure(e) =>
          throw e
      }
    case PeerClosed =>
      context.parent ! Hub.Unregister(clientId)
      context.stop(self)
  }


//def readMessage: IO.Iteratee[ByteString] =
// for {
//    lengthBytes <- take(4)
//    len = ascii(lengthBytes).toInt
//    bytes <- take(len)
// } yield {
//    bytes
// }
//}
//def printMessage: IO.Iteratee[Unit] =
//   repeat {
//      for {
//        string <- readMessage
//      }
//      yield {
//    val theBytes: Array[Byte] = string.toArray
//    val theMessage = ZombieSighting.defaultInstance.mergeFrom(theBytes)
//        println("name:" +theMessage.name)
//     }
// }


  def handleReceivedData(data: ByteString) {
    log.debug("ReceivedData")
    Common.Request.deserializeFromByteString(data) match {
      case Success(req) =>
        req.request match {
          case Common.CLIENT_INIT =>
            clientName = req("name").asInstanceOf[String]
          case Common.CLIENT_MESSAGE =>
            val msg = req("msg").asInstanceOf[String]
            context.parent ! Hub.Broadcast(clientId, clientName, msg)
        }
      case Failure(e) =>
        throw e
    }
  }

}
