defmodule Websocket.Move do

def move(%Websocket.Simple{id: id, map: map},x,y,state = %Websocket.State{maps: maps})  do
  mapDict=:array.get(map,maps)
  now=Lib.munixtime()
  case :dict.find(id,mapDict) do
    {:ok, record=%Websocket.User{lastAction: lastAction, user: user}} when (now-lastAction)>349 ->
      newMaps=:array.set(map,:dict.store(id,%Websocket.User{lastAction: now, x: x, y: y},mapDict),maps)
      Websocket.EsWebsock.sendToAll(Websocket.Worker, mapDict,id,["move @@@ ",user,"||",x,"||",y])
      {:noreply,%Websocket.State{maps: newMaps}}
    _ -> {:noreply,state}
  end
end

end
