defmodule WebserverSupervisor do
use Supervisor

# admin api
def start_link do
    IO.puts "WebserverSupervisor.start_link"
    Supervisor.start_link(__MODULE__,:ok)
end

#@riakworker Riak
@webworker WebserverWorker
@es_websock WebsocketWorker
@q_consumer WebsocketQConsumer
@users WebsocketUsers

# behaviour callbacks
def init(:ok) do
    IO.puts "WebserverSupervisor.init"

    children = [
      #worker(Riak.Client, []),
      worker(WebserverWorker, [[name: @webworker]]),
      worker(WebsocketEsWebsock, [[name: @es_websock]]), 
      worker(WebsocketQConsumer, [[name: @q_consumer]]), 
      worker(WebsocketUsers, [[name: @users]]), 

      #, [restart: :permanent, shutdown: 1000])
    ]

    supervise(children, strategy: :one_for_one)
end

end
