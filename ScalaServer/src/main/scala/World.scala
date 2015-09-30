package com.example.akkaTcpChat

import akka.actor.Actor
import akka.actor.ActorRef
import akka.actor.Props
import akka.event.Logging
import scala.collection.mutable.Map

object World {
  def props(): Props = {
    Props(new World())
  }
}

case class Get(w:Int, l:Int)
case class Set(w:Int, l:Int, h:Int ,v:Int) 

class World() extends Actor {
  import context.system
  var log = Logging(context.system, this)
  val chunks = Map.empty[(Int,Int), ActorRef]

  def chunk(x:Int, y:Int) = {
    val loc = (x/MapChunk.chunkSize, y/MapChunk.chunkSize) 
    if ( ! (chunks contains loc)) {
      chunks += (loc -> system.actorOf(Props(new MapChunk(x,y))))
    }
    chunks(loc)
  }

  def receive = {
    case "init" => init
    case "dump" => chunks.values.foreach(c => c ! "dump")
    case Get(w,l) => chunk(w,l) ! new Get(w,l)
    case Set(w,l,h,v) => chunk(w,l) ! new Set(w,l,h,v)
    case _ => log.info("World received unknown message")
  }

  def init = {
    log.info("World init")
    chunks += ((1,1) -> system.actorOf(Props(new MapChunk(1,1))))
  }
}
