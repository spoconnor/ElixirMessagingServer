package com.example.akkaTcpChat

// TcpHandler.scala

import java.net.InetSocketAddress
import akka.actor._
import akka.io.Tcp.Write
import akka.io.{ IO, Tcp }
import akka.util.ByteString

  //#simple-echo-handler
  class TcpHandler(connection: ActorRef, remote: InetSocketAddress)
    extends Actor with ActorLogging {

    import Tcp._
    import context.system
    log.info("TcpHandler: initializing")

    val messageHandler = context.actorOf(MessageHandler.props(), "messageHandler")

    // sign death pact: this actor terminates when connection breaks
    context watch connection

    case object Ack extends Event

    def receive = {
      case Received(data) =>
        log.info("TcpHandler: received data...")
        buffer(data)
        val msgLength:Int = data.head
        log.info("TcpHandler: received length {}", msgLength)
        val msgArray = data.slice(1, msgLength+1).toArray
        val msg = CommsMessages.Message.parseFrom(msgArray)
        log.info("TcpHandler: received msgtype {}", msg.msgtype)
        messageHandler ! msg
        connection ! Write(data, Ack)

        context.become({
          case Received(data) => buffer(data)
          case Ack            => acknowledge()
          case PeerClosed     => closing = true
        }, discardOld = false)

      case PeerClosed =>
        log.info("TcpHandler: peer closed")
        context stop self
    }

    //#storage-omitted
    override def postStop(): Unit = {
      log.info(s"transferred $transferred bytes from/to [$remote]")
    }

    var storage = Vector.empty[ByteString]
    var stored = 0L
    var transferred = 0L
    var closing = false

    val maxStored = 100000000L
    val highWatermark = maxStored * 5 / 10
    val lowWatermark = maxStored * 3 / 10
    var suspended = false

    //#simple-helpers
    private def buffer(data: ByteString): Unit = {
      storage :+= data
      stored += data.size

      if (stored > maxStored) {
        log.warning(s"drop connection to [$remote] (buffer overrun)")
        context stop self

      } else if (stored > highWatermark) {
        log.debug(s"suspending reading")
        connection ! SuspendReading
        suspended = true
      }
    }

    private def acknowledge(): Unit = {
      require(storage.nonEmpty, "storage was empty")

      val size = storage(0).size
      stored -= size
      transferred += size

      storage = storage drop 1

      if (suspended && stored < lowWatermark) {
        log.debug("resuming reading")
        connection ! ResumeReading
        suspended = false
      }

      if (storage.isEmpty) {
        if (closing) context stop self
        else context.unbecome()
      } else connection ! Write(storage(0), Ack)
    }
    //#simple-helpers
    //#storage-omitted
  }
//
