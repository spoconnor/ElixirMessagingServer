defmodule Websocket.CheckUser do

def checkUser(record = %Websocket.User{ip: ip}, state = %Websocket.State{banned: banned}) do
  Lib.trace("Check User #{ip2str(ip)}")
  case :lists.any(fn(IP1) -> IP1===IP end, banned) do
    :false -> checkUser1(ip,record,state)
    :true -> Lib.trace("fail at IP ban") 
             {:noreply,state}
  end
end

def ip2str(ip) do
  "#{elem(ip,0)}.#{elem(ip,1)}.#{elem(ip,2)}.#{elem(ip,3)}"
end

def checkUser1(ip,record = %Websocket.User{ip: ip}, state = %Websocket.State{lookupByIP: lbip}) do
  Lib.trace("CheckUser 1")
  case :gb_trees.lookup(ip,lbip) do
    none -> checkUser2(ip,record,state);
    {value,_} ->  Lib.trace("fail at already logged in")
                  {:reply,:fail,state}
  end
end

def checkUser2(ip,record,state) do
  Lib.trace("CheckUser 2")
  %Websocket.User{user: user, x: x, y: y, sprite: sprite, sock: sock, auth: auth} = record
  %Websocket.State{maps: maps, increment: id, lookupByID: lbid, lookupByName: lbName, lookupByIP: lbip} = state
  map0Dict=:array.get(0,maps)

  # TODO - move else where?
  # send all user locations for current map
  #gather =
  #  fn(_, %Websocket.User{x: x1, y: y1, sprite: sprite1, auth: auth1, user: user1}, acc)->
  #     [[",[\"",user1,"\",\"",sprite1,"\",\"",auth1,"\",\"",x1,"\",\"",y1,"\"]"]|acc]
  #  end
  #case :lists.flatten(:dict.fold(gather,[],map0Dict)) do
  #  [_|out] -> :nil
  #  [] -> out="[]"
  #end
  #Websocket.Websockets.msg(sock,"all",["[",out,"]"])
  #Websocket.Websockets.msg(sock,"all",out)

  newDict=:dict.store(id, %Websocket.User{id: id},map0Dict)
  maps1=:array.set(0,newDict,maps)
  lbid1=:dict.store(id,0,lbid)
  lbip1=:gb_trees.enter(ip,{0,id},lbip)
  lbName1=:gb_trees.enter(user,{0,id},lbName)
  #:array.foldl(fn(_,dict,_) -> Websocket.EsWebsock.sendToAll(Websocket.Worker, dict,id,["login @@@ ",user,"||",sprite,"||",auth,"||",x,"||",y]) end,0,maps)
  {:reply,id,%Websocket.State{maps: maps1, increment: id+1, lookupByID: lbid1, lookupByIP: lbip1, lookupByName: lbName1}}
end

end
