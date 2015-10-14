defmodule WorldServerListener do
  use GenServer

  def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
  end

  def init(state) do
    Lib.trace("Starting WorldServerListener")
    {:ok, state}
  end

  def terminate(reason, state) do
  end


end

