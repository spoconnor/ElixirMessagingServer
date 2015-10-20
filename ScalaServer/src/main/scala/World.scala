package com.example.akkaTcpChat

import java.net.InetSocketAddress
import akka.actor.{Actor, Props, ActorRef, ActorSystem}
import akka.event.Logging
import scala.collection.mutable.Map

object World {
  def props(actorSystem: ActorSystem): Props = {
    Props(new World(actorSystem))
  }
}

case class Get(w:Int, l:Int)
case class Set(w:Int, l:Int, h:Int ,v:Int) 

class World(actorSystem: ActorSystem) extends Actor {
  import context.system
  var log = Logging(context.system, this)
  lazy val webServerAddr = { new InetSocketAddress("localhost", 8083) }
  val chunks = Map.empty[(Int,Int), ActorRef]

  val tcpClient = actorSystem.actorOf(Props(new TcpClient(webServerAddr)))

  def chunk(x:Int, y:Int) = {
    chunks.getOrElseUpdate(
       (x/MapChunk.chunkSize, y/MapChunk.chunkSize),
       system.actorOf(Props(new MapChunk(x,y,tcpClient))))
  }

  def receive = {
    case "init" => init
    case "dump" => dump
    case Get(w,l) => chunk(w,l) ! new Get(w,l)
    case Set(w,l,h,v) => chunk(w,l) ! new Set(w,l,h,v)
    case _ => log.info("World received unknown message")
  }

  def dump = {
    log.info("Dump:")
    chunks.keys.foreach(k => log.info(k.toString))
    chunks.values.foreach(c => c ! "dump")
  }

  def init = {
    log.info("World init")
  }
}
