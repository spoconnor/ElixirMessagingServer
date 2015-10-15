defmodule WorldServerClient do
  use Connection

  @initial_state %{socket: nil, queue: :queue.new()}

  def start_link(opts \\ []) do
    Connection.start_link(__MODULE__, @initial_state)
  end

  def init(state) do
    {:connect, nil, state}
  end

  def connect(_info, state) do
    opts = [:binary, {:active, :once}]
    case :gen_tcp.connect({127,0,0,1}, 8842, opts) do
      {:ok, socket} ->
        {:ok, %{state | socket: socket}}
      {:error, reason} ->
        Lib.trace("WorldServerClient.init: Connection failed - #{inspect reason}")
        {:backoff, 5000, state} # try again in 5 seconds
    end
  end

  def send(server, msg) do
    GenServer.call(server, {:send,msg})
  end

  def handle_call({:send,msg}, from, %{socket: socket, queue: q} = state) do
    Lib.trace("WorldServerClient: Sending message to World server...")
    :ok = :gen_tcp.send(socket, msg)
    # enqueue client to send reponse to
    state = %{state | queue: :queue.in(from, q)}
    {:noreply, state} # dont reply yet
  end

  def handle_info({:tcp, socket, msg}, %{socket: socket} = state) do
    # Allow the socket to send us the next message. avoids client being flooded
    :inet.setopts(socket, active: :once)
    {{:value, client}, new_queue} = :queue.out(state.queue)
    # Reply to the correct client
    GenServer.reply(client, msg)
    {:noreply, %{state | queue: new_queue}}
  end

  def handle_info({:tcp_closed, socket}, %{socket: socket} = state) do
    Lib.trace("WorldServerClient: Connection closed")
    {:noreply, state}
  end
end

