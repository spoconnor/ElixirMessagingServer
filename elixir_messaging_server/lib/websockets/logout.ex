defmodule Websocket.Logout do

def logout(%Websocket.Simple{id: id, map: map},state = %Websocket.State{maps: maps, lookupByID: lbid, lookupByName: lbName, lookupByIP: lbip}) do
  mapDict=:array.get(map,maps)
  case :dict.find(id,mapDict) do
    {:ok, %Websocket.User{user: user, pid: pid, ip: ip}} ->
        send pid, {:die,"Disconnected"}
        Websocket.EsWebsock.sendToAll(Websocket.Worker, mapDict,id,["logout @@@ ",user])
        lbid1=:dict.erase(id,lbid)
        lbip1=:gb_trees.delete_any(ip,lbip)
        lbName1=:gb_trees.delete_any(user,lbName)
        {:noreply, %Websocket.State{maps: :array.set(map, :dict.erase(id,mapDict),maps), lookupByID: lbid1, lookupByName: lbName1, lookupByIP: lbip1}}
    _ -> {:noreply,state}
  end
end

end
