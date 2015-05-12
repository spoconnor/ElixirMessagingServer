defmodule WorldServer do
  use GenServer

  def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  # remote = :world@zen
  def connect_remote(server, remote) do
    GenServer.cast(server, {:connectServer, remote})
  end

  def get_map(server) do
    GenServer.cast(server, {:getMap})
  end

  def init(:ok) do
    Lib.trace("Starting WorldServer")
    state = :nil
    {:ok, state}
  end

  def handle_cast({:connectServer, remote}, state) do
    Lib.trace("Pinging server...")
    :pang = Node.ping(remote)
    Lib.trace("Ping to #{remote} successful")
    Lib.trace("Connecting to server...")
    :true = Node.connect remote
    Lib.trace("Connection to #{remote} successful")
    {:noreply, remote}
  end

  def handle_cast(:getMap, state) do
    Lib.trace("Get Map")
    #newUsers = HashDict.put(users, user, notify_pid)
    func = fn -> IO.inspect Node.self end
    Node.spawn(state, func)
    {:noreply, state}
  end

end

