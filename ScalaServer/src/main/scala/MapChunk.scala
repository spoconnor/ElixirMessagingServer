package com.example.akkaTcpChat

import akka.actor.Actor
import akka.actor.Props
import akka.event.Logging

case class GetChunk(xc: Int, yc: Int)

class Column {
  val data = Map[Int, Int]()
  def Set(h:Int, v:Int) = data + (h -> v)
  def Get(h:Int):Int = data(h)

  def Dump() = { for ((k,v) <- data) Console.print("%s ", k) }
}

class MapChunk(xc: Int, yc: Int) extends Actor {
  import context.system
  val chunkX: Int = xc
  val chunkY: Int = yc
  val width: Int = 10
  val length: Int = 10
  val height: Int = 10
  var log = Logging(context.system, this)
  private val data = Array.ofDim[Column](width,length)

  def receive = {
    case "init" => log.info("init")
    case GetChunk(x,y) => getChunk(x,y)
    case "dump" => Dump()
    case _ => log.info("MapChunk received unknown message")
  }
  
  override def preStart() = {
    // Initialization code
    var perlin = new perlinNoise.PerlinNoise()
    //var noise = perlin.GenerateWhiteNoise(10,10)
    var noise = perlin.GetIntMap(width,length, 0, height ,3)
    //noise.Dump()

    val column = Map[Int, Int]()
    for (w <- 0 to width-1)
    {
      for (l <- 0 to length-1)
      {
        val c = new Column()
        c.Set(noise.Get(w,l), 1)
        data(w)(l) = c
      }
    }    
  }

  def getChunk(x: Int, y: Int) {
    log.info("Get " + x + "," + y)
  }

  override def toString(): String = "Chunk " + width + "," + length;

  def Dump() =
  {
    for (w <- 0 to width-1)
    {
      for (l <- 0 to length-1)
      {
        data(w)(l).Dump()
      }
      Console.println()
    }
  }
}
