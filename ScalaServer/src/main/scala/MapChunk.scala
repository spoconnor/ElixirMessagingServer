package com.example.akkaTcpChat

import akka.actor._
import akka.event.Logging
import scala.collection.mutable.Map
import akka.util.ByteString

class Column {
  val data = Map.empty[Int, Int]
  def Set(h:Int, v:Int) = data += (h -> v)
  def Get(h:Int):Int = data(h)

  def Dump() = { data foreach {case (key, value) => Console.print(key)}}
}

object MapChunk {
  def chunkSize() = 10
}

class MapChunk(xc: Int, yc: Int) extends Actor with ActorLogging {
  import context.system
  val chunkX: Int = xc
  val chunkY: Int = yc
  val width: Int = MapChunk.chunkSize()
  val length: Int = MapChunk.chunkSize()
  val height: Int = 10
  private val data = Array.ofDim[Column](width,length)

  def receive = {
    case "init" => log.info("init")
    case "dump" => Dump()
    case msg:CommsMessages.MapRequest => mapRequest(msg)
    case "GetVisible" => GetVisible()
    case Get(w,l) => get(w,l)
    case Set(w,l,h,v) => set(w,l,h,v) 
    case _ => log.info("MapChunk received unknown message")
  }
  
  override def preStart() = {
    log.info("MapChunk preStart (" + chunkX + "," + chunkY + ")")
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

  def get(w:Int, l:Int) {
    data(w)(l)
  }

  def set(w:Int, l:Int, h:Int, v:Int) {
    data(w)(l).Set(h,v)
    context.actorSelection("/user/tcpclient") ! "set"
  }

  override def toString(): String = "Chunk " + width + "," + length;

  def Dump() =
  {
    Console.println("MapChunk dump (" + chunkX + "," + chunkY + ")")
    for (l <- 0 to length-1) { Console.print("-") }
    Console.print("--")
    Console.println()
    for (w <- 0 to width-1)
    {
      Console.print("|")
      for (l <- 0 to length-1)
      {
        data(w)(l).Dump()
      }
      Console.print("|")
      Console.println()
    }
    for (l <- 0 to length-1) { Console.print("-") }
    Console.print("--")
    Console.println()
  }

  def GetVisible() =
  {
    Console.println("MapChunk getting visible (" + chunkX + "," + chunkY + ")")
    context.actorSelection("/user/tcpclient") ! "visible"
  }

  def mapRequest(msg:CommsMessages.MapRequest) =
  {
    log.info("MapChunk.mapRequest")
    val response = new CommsMessages.Message(CommsMessages.MsgType.eMap)
//TODO - map coords
    val minX:Int = 0
    val minY:Int = 0
    val maxX:Int = MapChunk.chunkSize() 
    val maxY:Int = MapChunk.chunkSize()
    val dataSize:Int = 0
    response.setMap(new CommsMessages.Map(minX,minY,maxX,maxY,dataSize))
    // TODO move common code to fn
    val msgBytes = response.toByteArray()
    val resBytes = ByteString("%04d%s".format(
      response.getSerializedSize,
      new String(msgBytes)))
    context.actorSelection("/user/tcpclient") ! resBytes
  }
}
