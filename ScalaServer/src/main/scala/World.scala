package com.example.akkaTcpChat

import java.net.InetSocketAddress
import akka.actor._
import akka.event.Logging
import scala.collection.mutable.Map

object World {
  def props(): Props = {
    Props(new World())
  }
}

case class Get(w:Int, l:Int)
case class Set(w:Int, l:Int, h:Int ,v:Int) 

class World() extends Actor with ActorLogging {
  import context.system
  val chunks = Map.empty[(Int,Int), ActorRef]

  def chunk(x:Int, y:Int) = {
    chunks.getOrElseUpdate(
       (x/MapChunk.chunkSize, y/MapChunk.chunkSize),
       system.actorOf(Props(new MapChunk(x,y))))
  }

  def receive = {
    case "init" => init
    case "dump" => dump
    case msg:CommsMessages.MapRequest => 
      log.info("World recevied maprequest")
      chunk(msg.x,msg.y) ! msg
    case "GetVisible" => GetVisible
    case Get(w,l) => chunk(w,l) ! new Get(w,l)
    case Set(w,l,h,v) => chunk(w,l) ! new Set(w,l,h,v)
    case _ => log.info("World received unknown message")
  }

  def dump = {
    log.info("Dump:")
    chunks.keys.foreach(k => log.info(k.toString))
    chunks.values.foreach(c => c ! "dump")
  }

  def GetVisible = {
    log.info("GetVisible:")
    chunks.keys.foreach(k => log.info(k.toString))
    chunks.values.foreach(c => c ! "GetVisible")
  }

  def init = {
    log.info("World init")
  }
}
