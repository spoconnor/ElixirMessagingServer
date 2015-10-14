defmodule WorldServerListener do
  use GenServer

  def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  def send(server, msg) do
    GenServer.cast(server, {:send,msg})
  end

  def init(state) do
    Lib.trace("Starting WorldServerListener")
    Lib.trace("WorldServerListener.connect")
    opts = [:binary, {:active, true}]
    {:ok, socket} = :gen_tcp.connect({127,0,0,1}, 8842, opts)
    {:ok, %{state | socket: socket}}
  end

  def terminate(reason, state) do
    :gen_tcp.close(state)  
  end

  def handle_cast({:send,msg}, %{socket: socket} = state) do
    Lib.trace("Sending message to World server...")
    :gen_tcp.send(socket, msg)
  end

end

