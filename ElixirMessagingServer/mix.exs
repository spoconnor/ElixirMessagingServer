defmodule ElixirMessagingServer.Mixfile do
  use Mix.Project

  def project do
    [app: :ElixirMessagingServer,
     version: "0.0.1",
     elixir: "~> 1.0.0",
     deps: deps]
  end

  # Configuration for the OTP application
  #
  # Type `mix help compile.app` for more information
  def application do
    [applications: 
      [:logger, :kernel, :stdlib, :lager, :ranch, :cowlib, :cowboy, :jiffy, :ssl, :ibrowse, :inets, :crypto, :riakc ],
      mod: {ElixirMessagingServer, []},
      env: [
        http_port: 8080,
        http_listener_count: 10
      ],
    ]
  end

  defp deps do
    [
      {:cowboy, github: "extend/cowboy"},
      {:lager, github: "basho/lager" },
      {:jiffy, github: "davisp/jiffy" },
      {:ibrowse, github: "cmullaparthi/ibrowse" },
      {:riak_pb, github: "basho/riak_pb", override: true, tag: "2.0.0.16", compile: "./rebar get-deps compile deps_dir=../"},
      {:riakc, github: "basho/riak-erlang-client"},
      {:eper, github: "massemanet/eper" },
      {:mixer, github: "opscode/mixer" },
      {:sync, github: "rustyio/sync" },
      {:riakc, github: "basho/riak-erlang-client" },
      {:exprotobuf, github: "bitwalker/exprotobuf", tag: "0.8.3"},
      {:gpb, github: "tomas-abrahamsson/gpb", tag: "3.16.0", override: :true},
      {:amqp, github: "pma/amqp", tag: "v0.0.6" },
    ]
  end
end
