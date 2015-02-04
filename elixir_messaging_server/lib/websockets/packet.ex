defmodule Packet do

  def decode(<<a::8,b::8,body::binary>>) do
    header = CommsMessages.Header.decode(<<a,b>>)
    case header.msgtype do
      1 -> {header,CommsMessages.Ping.decode(body)}
      2 -> {header,CommsMessages.Register.decode(body)}
      3 -> {header,CommsMessages.Registered.decode(body)}
      4 -> {header,CommsMessages.Say.decode(body)}
      5 -> {header,CommsMessages.Movement.decode(body)}
      6 -> {header,CommsMessages.Action.decode(body)}
      7 -> {header,CommsMessages.Block.decode(body)}
    end
  end

  def msgType(<<a::8,b::8,body::binary>>) do
    header = CommsMessages.Header.decode(<<a,b>>)
    case header.msgtype do
      1 -> "Ping"
      2 -> "Register"
      3 -> "Registered"
      4 -> "Say"
      5 -> "Movement"
      6 -> "Action"
      7 -> "Block"
    end
  end

  def encode(message) do
    case message.__struct__ do
      CommsMessages.Ping -> 
        msgtype = 1
        body = CommsMessages.Ping.encode(message)
      CommsMessages.Register -> 
        msgtype = 2
        body = CommsMessages.Register.encode(message)
      CommsMessages.Registered -> 
        msgtype = 3
        body = CommsMessages.Registered.encode(message)
      CommsMessages.Say -> 
        msgtype = 4
        body = CommsMessages.Say.encode(message)
      CommsMessages.Movement -> 
        msgtype = 5
        body = CommsMessages.Movement.encode(message)
      CommsMessages.Action -> 
        msgtype = 6
        body = CommsMessages.Action.encode(message)
      CommsMessages.Block -> 
        msgtype = 7
        body = CommsMessages.Block.encode(message)
    end
    CommsMessages.Header.encode(CommsMessages.Header.new(msgtype: msgtype)) <> body
  end

end

