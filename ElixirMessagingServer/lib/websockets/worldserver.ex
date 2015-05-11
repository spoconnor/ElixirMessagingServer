defmodule WorldServer do
  use GenServer

  def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  def connect_server() do
    GenServer.cast(server, {:connectServer})
  end

  def get_map(server) do
    GenServer.cast(server, {:getmap})
  end

  def init(:ok) do
    Lib.trace("Starting WorldServer")
    {:ok}
  end

  def handle_cast({:connectServer}, worldServer) do
    Lib.trace("Pinging server world@zen...")
    :pong = Node.ping(:world@zen)
    Lib.trace("Ping to world@zen successful")
    {:noreply, worldServer}
  end

  def handle_cast({:add, user, notify_pid}, users) do
    Lib.trace("Adding user #{user}")
    newUsers = HashDict.put(users, user, notify_pid)
    {:noreply, newUsers}
  end

  def handle_cast({:notify, payload}, users) do
    #todo
    Lib.trace("Notifying users", payload)
    {header,data} = Packet.decode(payload)
    Lib.trace("MessageType:", header.msgtype)
#    actions(payload, msg)
    notify_users(payload, header.dest, users)
    {:noreply, users}
  end

  # Send to all users
  defp notify_users(payload, "", users) do
    Enum.each users, fn {user, notify_pid} -> 
      Lib.trace("Sending notify to #{user}")
      send notify_pid, payload
    end
    {:noreply, users}
  end

  # Send to a specified user
  defp notify_users(payload, dest, users) do
    {user, notify_pid} = Enum.find(users, fn {user, _notify_pid} -> user == dest end)
    Lib.trace("Sending notify to #{user}")
    send notify_pid, payload
    {:noreply, users}
  end

end

