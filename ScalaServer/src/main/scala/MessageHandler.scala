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
    case 6 => processSay(Option[Say])
    case 7 => processMapRequest(Option[MapRequest])
    case 10 => processMapUpdate(Option[MapUpdate])
    case _ => log.debug("MessageHandler.process: Ignoring message")
  }

  def processSay(say:CommsMessages.Say) = {
      log.debug("MessageHandler.processSay:" + say.text)
  }
  def processSay(say:None) = {
  }

  def processMapRequest(mapRequest:CommsMessages.MapRequest) = {
      log.debug("MessageHandler.mapRequest:" + mapRequest.x + "," + mapRequest.y)
  }
  def processSay(mapRequest:None) = {
  }

  def processMapUpdate(mapUpdate:CommsMessages.MapUpdate) = {
      log.debug("MessageHandler.mapUpdate:" + mapUpdate.x + "," + mapUpdate.y)
  }
  def processSay(mapUpdate:None) = {
  }
}
