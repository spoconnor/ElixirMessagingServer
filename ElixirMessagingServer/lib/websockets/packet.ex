defmodule Packet do

  def decode(<<dataSize::8,data::binary>>) do
    msgData = :binary.part(data,0,dataSize)
    message = CommsMessages.Message.decode(<<msgData::binary>>)
  end

  def msgType(<<dataSize::8,data::binary>>) do
    msgData = :binary.part(data,0,dataSize)
    message = CommsMessages.Message.decode(<<msgData::binary>>)
    case message.msgtype do
      1 -> "Response"
      2 -> "Ping"
      3 -> "Pong"
      4 -> "NewUser"
      5 -> "Login"
      6 -> "Say"
    end
  end

  def encode(message) do
    bodyData = CommsMessages.Message.encode(message)
    <<byte_size(bodyData)>> <> bodyData
  end

end

