defmodule WebsocketConnect do
use Bitwise

defmacro timeoutTime do
  30*1000
end
defmacro idleTime do
 60*10*1000
end

def accept_connections(s) do
  Lib.trace("Accept_Connections")
  {:ok, clientS} = :gen_tcp.accept(s)
  spawn(fn() -> accept_connections(s) end)
  receive do
    {_tcp,_,bin} ->
      reply =  WebsocketWebsockets.handshake(bin)
      Lib.trace("Reply: #{reply} to '#{:erlang.port_info(clientS)[:id]}'")
      Lib.trace(:erlang.port_info(clientS))
      :gen_tcp.send(clientS, reply)
      #WebsocketWebsockets.sendTcpMsg(clientS, reply)
      step2(clientS)
    after timeoutTime ->
      WebsocketWebsockets.die(clientS, "Timeout on Handshake")
  end
end

def step2(clientS) do
  Lib.trace("Connection Step2")
  receive do
    {_tcp, _, bin1} ->
      str = to_string(decodeString(bin1))
      Lib.trace("Received:", str)
      {header,body} = Packet.decode(str)
      registerMsg(clientS, header, body)
    after timeoutTime ->
      WebsocketWebsockets.die(clientS,"Timeout on Handshake")
  end
end

# RegisterClient
def registerMsg(clientS, header, register) do
  Lib.trace("MsgType:", header.msgtype)
  Lib.trace("Registering #{register.name}")
  #if (length(user.name)>25) do
  #  WebsocketWebsockets.die("Name too long")
  #end
  {:ok,{ip,_}} = :inet.peername(clientS)
  state = %WebsocketUser{user: register.name, sock: clientS, x: 1,y: 1, ip: ip, pid: self()}

  notify_pid = spawn(fn() -> notify_thread(clientS) end)
  WebsocketUsers.add_user(WebsocketUsers, register.name, notify_pid)

  case WebsocketEsWebsock.checkUser(WebsocketWorker, state) do
    {:fail, _} -> WebsocketWebsockets.die(clientS,"Already Connected");
    id ->
      Lib.trace("ObjectId: #{id}")
      registered = CommsMessages.Registered.new(objectid: id, motd: "Welcome!")
      data = Packet.encode(registered)
      WebsocketWebsockets.sendTcpMsg(clientS, data)
      client(%WebsocketSimple{id: id, sock: clientS})
  end
end

#def registerMsg(clientS, unexpected, _data) do
#  Lib.trace("Unexpected type received", unexpected)
#end

def decodeString(data) do
  decodeStream(:binary.bin_to_list(data))
end
# encrypted string marker
def decodeStream([129,b2|t]) do
  #length = b2 &&& 127  # bitwise AND, unless special case
  #indexFirstMask = 2   # if not a special case
  # TODO - For message length > 126 bytes
  #if (length == 126)   # if a special case, change indexFirstMask
  #  indexFirstMask = 4
  #else if length == 127  # special case 2
  #  indexFirstMask = 10
  #end
  [mask1,mask2,mask3,mask4|data] = t
  masks = [mask1,mask2,mask3,mask4]
  decodeBytes(data,masks,[])
end

def decodeBytes([],_masks,decoded) do
  decoded
end
def decodeBytes(data,masks,decoded) do
  [byte|data2]=data
  [mask|masks2]=masks
  decodeBytes(data2, masks2++[mask], decoded ++ [byte ^^^ mask])
end

def client(state) do
  Lib.trace("Client #{state.id} receive loop")
  receive do
    {_tcp,_,bin} -> 
      str = to_string(decodeString(bin))
      Lib.trace("received:", str)
      Lib.trace("type:", Packet.msgType(str))
      # Send message thru rabbit queue

      {:ok, conn} = AMQP.Connection.open("amqp://guest:guest@localhost")
      {:ok, chan} = AMQP.Channel.open(conn)
      #{:ok, _queue} = AMQP.Queue.declare( chan, Globals.send_queue, [auto_delete: true, durable: false, exclusive: false])
      #:ok = AMQP.Exchange.declare(chan, Globals.mq_exchange, :direct, [auto_delete: true, durable: false])
      #:ok = AMQP.Queue.bind chan, Globals.send_queue, Globals.mq_exchange
      :ok = AMQP.Basic.publish(chan, Globals.mq_exchange, "Inbound", str)

      client(state)
    {:tcp_closed,_} ->
      logoutAndDie(state,"Disconnected")
    {:die,reason} ->
      logoutAndDie(state,reason)
    what ->
      Lib.trace(what)
      logoutAndDie(state,"Crash")
    after idleTime ->
      logoutAndDie(state,"Idle")
  end
end

def notify_thread(clientS) do
  Lib.trace("Client notify_thread")
  receive do
    data ->
      Lib.trace("Client notify thread recd", data)
      WebsocketWebsockets.sendTcpMsg(clientS, data)
      notify_thread(clientS)
  end
end

def logoutAndDie(state,msg) do
    WebsocketEsWebsock.logout(WebsocketWorker, state)
    WebsocketWebsockets.die(state.sock,msg)
end
    
end
