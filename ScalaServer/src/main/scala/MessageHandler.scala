package com.example.akkaTcpChat

import akka.actor._
import akka.io.{IO, Tcp}
import akka.event.Logging

object MessageHandler {

  def props(): Props = {
    Props(new MessageHandler())
  }
}

class MessageHandler() extends Actor with ActorLogging {
  import context.system
  log.info("MessageHandler initialized")

  def receive = {
    case msg:CommsMessages.Message =>
      log.info("MessageHandler: Processing " + msg.msgtype)
      process(msg)
    case _ =>
      log.info("MessageHandler: Unknown message")
  }

  def process(msg:CommsMessages.Message) = {
    msg.msgtype match {
      case 6 => msg.say match { 
        case Some(m) => processSay(m)
        case _ => log.info("MessageHandler.process: Expected data missing")
      }
      case 7 => msg.mapRequest match { 
        case Some(m) => processMapRequest(m) 
        case _ => log.info("MessageHandler.process: Expected data missing")
      }
      case 10 => msg.mapUpdate match { 
        case Some(m) => processMapUpdate(m) 
        case _ => log.info("MessageHandler.process: Expected data missing")
      }
      case _ => log.info("MessageHandler.process: Ignoring message")
    }
  }

  def processSay(say:CommsMessages.Say) = {
    log.info("MessageHandler.processSay:" + say.text)
  }

  def processMapRequest(mapRequest:CommsMessages.MapRequest) = {
    log.info("MessageHandler.mapRequest:" + mapRequest.x + "," + mapRequest.y)
    context.actorSelection("/user/world") ! mapRequest
  }

  def processMapUpdate(mapUpdate:CommsMessages.MapUpdate) = {
    log.info("MessageHandler.mapUpdate:" + mapUpdate.x + "," + mapUpdate.y)
  }
}
