defmodule LibTest do
  use ExUnit.Case

  # A simple test
  test "Floor" do
    assert(Websocket.Lib.floor(5) == 5)
    assert(Websocket.Lib.floor(5.2) == 5)
    assert(Websocket.Lib.floor(5.7) == 5)
    assert(Websocket.Lib.floor(6.1) == 6)
  end

  

end
