defmodule WebserverWorker do
use GenServer

# Client API

 def start_link(opts \\ []) do
  IO.puts "Starting WebserverWorker link..."
  GenServer.start_link(__MODULE__, :ok, opts)
 end

 def stop() do
  GenServer.call(:stop)
 end

# Server Callbacks

 def init(:ok) do
  IO.puts "Starting cowboy worker..."

  port = Application.get_env(:ElixirMessagingServer, :http_port)
  listenerCount = Application.get_env(:ElixirMessagingServer, :http_listener_count)
  IO.puts("Listening on port #{port}")

  dispatch =
    :cowboy_router.compile([
       {
         :_,
         [
            {"/events", WebserverEventsHandler, []},
            {"/foobar", WebserverFoobarHandler, []},
            {"/api", WebserverRestApiHandler, []},
            #{"/ws", :cowboy_static, {:priv_file, :ElixirMessagingServer, "ws_index.html"}},
            #{"/websocket", WebserverWebsocketHandler, []},
            {"/static/[...]", :cowboy_static, {:priv_dir, :ElixirMessagingServer, "static"}},
            #{"/api/[:id]", [{:v1, :int}], WebserverToppageHandler, []},
            {"/[...]", :cowboy_static, {:file, "priv/index.html"}},
         ]
       }
    ])
  ranchOptions =
    [ 
      {:port, port}
    ]
  cowboyOptions =
    [ 
      {:env, [
         {:dispatch, dispatch}
      ]},
      {:compress,  true},
      {:timeout,   12000}
    ]
    
  {:ok, _} = :cowboy.start_http(:http, listenerCount, ranchOptions, cowboyOptions)

  {:ok, {}}
 end

 def handle_call(:stop, _from, state) do
  {:stop, :normal, :ok, state}
 end

end


