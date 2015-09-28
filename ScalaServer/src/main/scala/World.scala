package com.example.akkaTcpChat

import akka.actor.Actor
import akka.actor.ActorRef
import akka.actor.Props
import akka.event.Logging

object World {
  def props(): Props = {
    Props(new World())
  }
}

case class ChunkLoc(x:Int, y:Int)
case class Get(w:Int, l:Int)
case class Set(w:Int, l:Int, h:Int ,v:Int) 

class World() extends Actor {
  import context.system
  var log = Logging(context.system, this)
  //val chunk = system.actorOf(Props[MapChunk], name = "chunk1")
  val chunks = scala.collection.mutable.Map.empty[ChunkLoc, ActorRef]

  def receive = {
    case "init" => init
    case Get(w,l) => chunks(new ChunkLoc(1,1)) ! new Get(w,l)
    case Set(w,l,h,v) => chunks(new ChunkLoc(1,1)) ! new Set(w,l,h,v)
    case "dump" => chunks(new ChunkLoc(1,1)) ! "dump"
    case _ => log.info("World received unknown message")
  }

  def init = {
    log.info("World init")
    chunks += (new ChunkLoc(1,1) -> system.actorOf(Props(new MapChunk(1,1))))
  }
}
