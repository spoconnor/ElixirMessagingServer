defmodule WorldServerClient do
  use Connection

  @initial_state %{socket: nil}

  def start_link(opts \\ []) do
    Connection.start_link(__MODULE__, @initial_state)
  end

  def init(state) do
    {:connect, nil, state}
  end


  def send(server, msg) do
    Lib.trace("WorldServerClient send")
    Connection.call(server, {:send,msg})
  end

  def recv(server, bytes, timeout \\ 3000) do
    Lib.trace("WorldServerClient recv")
    Connection.call(server, {:recv,bytes,timeout})
  end

  def close(server) do
    Connection.call(server, :close)
  end


  def connect(_info, state) do
    opts = [:binary, {:active, :false}]
    case :gen_tcp.connect({127,0,0,1}, 8842, opts) do
      {:ok, socket} ->
        Lib.trace("WorldServerClient.init: Connected")
        {:ok, %{state | socket: socket}}
      {:error, reason} ->
        Lib.trace("WorldServerClient.init: Connection failed - #{inspect reason}")
        {:backoff, 5000, state} # try again in 5 seconds
    end
  end

 def disconnect(info, %{socket: socket} = state) do
    :ok = :gen_tcp.close(socket)
    case info do
      {:close, from} ->
        Connection.reply(from, :ok)
      {:error, :closed} ->
        Lib.trace("WorldServerClient Connection closed")
      {:error, reason} ->
        reason = :inet.format_error(reason)
        Lib.trace("WorldServerClient Connection error - #{inspect reason}")
    end
    {:connect, :reconnect, %{state | socket: nil}}
  end


  def handle_call(_, _, %{socket: nil} = state) do
    Lib.trace("WorldServerClient connection is closed")
    {:reply, {:error, :closed}, state}
  end
  def handle_call({:send, data}, _, %{socket: socket} = state) do
    Lib.trace("WorldServerClient sending data")
    case :gen_tcp.send(socket, data) do
      :ok ->
        {:reply, :ok, state}
      {:error, _} = error ->
        {:disconnect, error, error, state}
    end
  end
  def handle_call({:recv, bytes, timeout}, _, %{socket: socket} = state) do
    Lib.trace("WorldServerClient waiting for recv")
    case :gen_tcp.recv(socket, bytes, timeout) do
      {:ok, _} = ok ->
        {:reply, ok, state}
      {:error, :timeout} = timeout ->
        {:reply, timeout, state}
      {:error, _} = error ->
        {:disconnect, error, error, state}
    end
  end
  def handle_call(:close, from, state) do
    {:disconnect, {:close, from}, state}
  end

end

