defmodule Packet do

  def decode(<<a::8,b::8,body::binary>>) do
    header = CommsMessages.Header.decode(<<a,b>>)
    case header.msgtype do
      1 -> {header,CommsMessages.Response.decode(body)}
      2 -> {header,CommsMessages.Ping.decode(body)}
      3 -> {header,CommsMessages.Pong.decode(body)}
      4 -> {header,CommsMessages.NewUser.decode(body)}
      5 -> {header,CommsMessages.Login.decode(body)}
      6 -> {header,CommsMessages.Say.decode(body)}
    end
  end

  def msgType(<<a::8,b::8,body::binary>>) do
    header = CommsMessages.Header.decode(<<a,b>>)
    case header.msgtype do
      1 -> "Response"
      2 -> "Ping"
      3 -> "Pong"
      4 -> "NewUser"
      5 -> "Login"
      6 -> "Say"
    end
  end

  def encode(message) do
    case message.__struct__ do
      CommsMessages.Response -> 
        msgtype = 1
        body = CommsMessages.Response.encode(message)
      CommsMessages.Ping -> 
        msgtype = 2
        body = CommsMessages.Ping.encode(message)
      CommsMessages.Pong -> 
        msgtype = 3
        body = CommsMessages.Pong.encode(message)
      CommsMessages.NewUser -> 
        msgtype = 4
        body = CommsMessages.NewUser.encode(message)
      CommsMessages.Login -> 
        msgtype = 5
        body = CommsMessages.Login.encode(message)
      CommsMessages.Say -> 
        msgtype = 6
        body = CommsMessages.Say.encode(message)
    end
    CommsMessages.Header.encode(CommsMessages.Header.new(msgtype: msgtype)) <> body
  end

end

