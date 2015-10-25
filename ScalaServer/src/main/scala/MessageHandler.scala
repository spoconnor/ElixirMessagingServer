package com.example.akkaTcpChat

import akka.actor.{Props, Actor}
import akka.io.{IO, Tcp}
import akka.event.Logging

object MessageHandler {

  def props(): Props = {
    Props(new MessageHandler())
  }
}

class MessageHandler() extends Actor {

  import context.system

  val log = Logging(context.system, this)

  def receive = {
    case msg:CommsMessages.Message =>
      log.debug("MessageHandler: Processing " + msg.msgtype)
      process(msg)
    case _ =>
      log.debug("MessageHandler: Unknown message")
  }

  def process(msg:CommsMessages.Message) = 
  msg.msgtype match {
    case 6 => msg.say match { 
      case Some(m) => processSay(m)
      case _ => log.debug("MessageHandler.process: Expected data missing")
    }
    case 7 => msg.mapRequest match { 
      case Some(m) => processMapRequest(m) 
      case _ => log.debug("MessageHandler.process: Expected data missing")
    }
    case 10 => msg.mapUpdate match { 
      case Some(m) => processMapUpdate(m) 
      case _ => log.debug("MessageHandler.process: Expected data missing")
    }
    case _ => log.debug("MessageHandler.process: Ignoring message")
  }

  def processSay(say:CommsMessages.Say) = {
      log.debug("MessageHandler.processSay:" + say.text)
  }

  def processMapRequest(mapRequest:CommsMessages.MapRequest) = {
      log.debug("MessageHandler.mapRequest:" + mapRequest.x + "," + mapRequest.y)
  }

  def processMapUpdate(mapUpdate:CommsMessages.MapUpdate) = {
      log.debug("MessageHandler.mapUpdate:" + mapUpdate.x + "," + mapUpdate.y)
  }
}
