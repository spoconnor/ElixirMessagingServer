defmodule Packet do

  def decode(header,body) do
    h = Poison.decode!(header, as: CommsMessages.Header)
    Lib.trace("Header Type: #{h.msgtype}")
    case h.msgtype do
      "Response" -> {:ok, Poison.decode!(body, as: CommsMessages.Response)}
      "Ping" -> {:ok, Poison.decode!(body, as: CommsMessages.Ping)}
      "Pong" -> {:ok, Poison.decode!(body, as: CommsMessages.Pong)}
      "NewUser" -> {:ok, Poison.decode!(body, as: CommsMessages.NewUser)}
      "Login" -> {:ok, Poison.decode!(body, as: CommsMessages.Login)}
      "Say" -> {:ok, Poison.decode!(body, as: CommsMessages.Say)}
    end
  end

  def encode(message, from, dest) do
    #JSON.encode(message)
  end

#  def decode(<<headerSize::8,data::binary>>) do
#    headerData = :binary.part(data,0,headerSize)
#    bodyData = :binary.part(data,headerSize,byte_size(data)-headerSize)
#    header = CommsMessages.Header.decode(<<headerData::binary>>)
#    case header.msgtype do
#      1 -> {header,CommsMessages.Response.decode(bodyData)}
#      2 -> {header,CommsMessages.Ping.decode(bodyData)}
#      3 -> {header,CommsMessages.Pong.decode(bodyData)}
#      4 -> {header,CommsMessages.NewUser.decode(bodyData)}
#      5 -> {header,CommsMessages.Login.decode(bodyData)}
#      6 -> {header,CommsMessages.Say.decode(bodyData)}
#    end
#  end

#  def msgType(<<headerSize::8,data::binary>>) do
#    headerData = :binary.part(data,0,headerSize)
#    header = CommsMessages.Header.decode(<<headerData::binary>>)
#    case header.msgtype do
#      1 -> "Response"
#      2 -> "Ping"
#      3 -> "Pong"
#      4 -> "NewUser"
#      5 -> "Login"
#      6 -> "Say"
#    end
#  end

#  def encode(message,from,dest) do
#    case message.__struct__ do
#      CommsMessages.Response -> 
#        msgtype = 1
#        bodyData = CommsMessages.Response.encode(message)
#      CommsMessages.Ping -> 
#        msgtype = 2
#        bodyData = CommsMessages.Ping.encode(message)
#      CommsMessages.Pong -> 
#        msgtype = 3
#        bodyData = CommsMessages.Pong.encode(message)
#      CommsMessages.NewUser -> 
#        msgtype = 4
#        bodyData = CommsMessages.NewUser.encode(message)
#      CommsMessages.Login -> 
#        msgtype = 5
#        bodyData = CommsMessages.Login.encode(message)
#      CommsMessages.Say -> 
#        msgtype = 6
#        bodyData = CommsMessages.Say.encode(message)
#    end
#    header = CommsMessages.Header.new(msgtype: msgtype, from: from, dest: dest)
#    headerData = CommsMessages.Header.encode(header)
#    <<byte_size(headerData)>> <> headerData <> bodyData
#  end

end

