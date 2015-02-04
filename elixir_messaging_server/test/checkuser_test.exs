defmodule CheckUserTest do
  use ExUnit.Case
  alias Websocket.User
  alias Websocket.State
  alias Websocket.CheckUser

  # Check User
  test "Check User" do

    state = %State{}
#      maps:  :array.new(2,{:default,:dict.new()}),
#      increment:  0,
#      lookupByID:  :dict.new(),
#      lookupByName:  :gb_trees.empty(),
#      lookupByIP:  :gb_trees.empty(),
#      banned:  [],
#      sock:  nil

    user = %User{ip: "10.10.10.1"}
#    x:  "0",
#    y:  "0",
#    id:  0,
#    user:  "",
#    map:  0,
#    lastMessage:  Lib.munixtime()-3000,
#    lastAction:  Lib.munixtime()-3000,
#    floodTest:  [ for x <- 1..6 do 4000 end ], 
#    sprite:  "0",
#    ip:  :nil,
#    auth:  "0",
#    sock:  :nil,
#    pid:  :nil

    CheckUser.checkUser(user, state)
  end
  

end
