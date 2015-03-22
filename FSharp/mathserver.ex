# ---------------------------------------------------------------------
# File: mathserver.ex
#
# This is a simple implementation of a gen_server callback module.

defmodule State do
  defstruct(
    temp: :nil
 )
end

defmodule Mathserver do
use GenServer

#export([init/1, handle_call/3, handle_cast/2, handle_info/2,
#         terminate/2, code_change/3]).
#         
#export([
#            start_link/0,
#            stop/0,
#            multiply/2
#            ]).
#
#define(SERVER, ?MODULE).

#
# API
#


#
# @doc Starts the server.
#
# @spec start_link() -> {ok, Pid}
# where
# Pid = pid()
# @end
#
def start_link(opts \\ []) do
    GenServer.start_link(__MODULE__, :ok, opts)
end

#
# @doc Stops the server.
# @spec stop() -> ok
# @end
#
def stop(server) do
    GenServer.call(server, :die)
end

#
# @doc multiplies two integers.
# @spec multiply(X::integer(), Y::integer()) -> {ok, Product}
# where
# Product = integer()
# @end
#
def multiply(server, x, y) do
    GenServer.call(server, {:multiply, {x, y}})
end

#
# gen_server callbacks
#


def init(:ok) do
    {:ok, %State{}}
end

def handle_call({:multiply, {x, y}}, _from, state) do
    reply = {:ok, multiply_impl(x,y)}
    {:reply, reply, state}
end
def handle_call(_request, _from, state) do
    reply = :ok
    {:reply, reply, state}
end

def handle_cast(_msg, state) do
    {:noreply, state}
end

def handle_info(_info, state) do
    {:noreply, state}
end

def terminate(_reason, _state) do
    :ok
end

def code_change(_oldVsn, state, _extra) do
    {:ok, state}
end

#
# API internals
#

def multiply_impl(first, second) do
    first * second
end

end # Module
