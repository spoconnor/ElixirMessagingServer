defmodule WorldServerClient do
  use GenServer

  def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  def connect(server) do
    GenServer.cast(server, :connect)
  end

  def send(server, msg) do
    GenServer.cast(server, {:send,msg})
  end

  def init(state) do
    {:ok, state}
  end

  def terminate(reason, state) do
    Lib.trace("WorldServerClient.terminate")
    #:gen_tcp.close(state)  
  end

  def handle_cast(:connect, state) do
    Lib.trace("WorldServerClient.init Initiating connection...")
    opts = [:binary, {:active, true}]
    case :gen_tcp.connect({127,0,0,1}, 8842, opts) do
      {:ok, socket} -> {:ok, %{state | socket: socket}}
      {:error, reason} ->
        Lib.trace("WorldServerClient.init: Connection failed - #{inspect reason}")
        {:error, reason}
    end
  end

  def handle_cast({:send,msg}, %{socket: socket} = state) do
    Lib.trace("WorldServerClient: Sending message to World server...")
    :gen_tcp.send(socket, msg)
  end

end

