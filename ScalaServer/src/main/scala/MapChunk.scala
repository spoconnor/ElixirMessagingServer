package com.example.akkaTcpChat

import akka.actor.Actor
import akka.actor.Props
import akka.event.Logging

case class GetChunk(xc: Int, yc: Int)

class Column {
  val data = Map[Int, Int]()
}

class MapChunk(xc: Int, yc: Int) extends Actor {
  import context.system
  val width: Int = xc
  val length: Int = yc
  var log = Logging(context.system, this)

  def receive = {
    case "init" => log.info("init")
    case GetChunk(x,y) => getChunk(x,y)
    case _ => log.info("MapChunk received unknown message")
  }
  
  override def preStart() = {
    // Initialization code
    var perlin = new perlinNoise.PerlinNoise()
    //var noise = perlin.GenerateWhiteNoise(10,10)
    var noise = perlin.GetIntMap(80, 30, 0, 9 ,3)
    //noise.Dump()

    val column = Map[Int, Int]()
    val data = Array.ofDim[Column](width,length)
    
  }

  def getChunk(x: Int, y: Int) {
    log.info("Get " + x + "," + y)
  }

  override def toString(): String = "Chunk " + width + "," + length;
}
