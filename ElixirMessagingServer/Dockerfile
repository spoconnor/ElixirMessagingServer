FROM debian:wheezy
MAINTAINER Sean OConnor <onewheel@gmail.com>

# Erlang
ADD http://packages.erlang-solutions.com/ubuntu/erlang_solutions.asc /tmp/install/
ADD http://packages.erlang-solutions.com/erlang-solutions_1.0_all.deb /tmp/install/
RUN apt-key add /tmp/install/erlang_solutions.asc \
 && dpkg -i /tmp/install/erlang-solutions_1.0_all.deb \
 && apt-get update \
 && apt-get install erlang wget unzip -y \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

# Elixir
RUN wget https://github.com/elixir-lang/elixir/releases/download/v1.1.0/Precompiled.zip \
 && unzip Precompiled.zip -d /usr \
 && rm Precompiled.zip

############
# My Code

RUN mkdir /opt/ElixirMessagingServer
WORKDIR /opt/ElixirMessagingServer
ADD ElixirMessagingServer/ElixirMessagingServer /opt/ElixirMessagingServer/
RUN mkdir /opt/ElixirMessagingServer/priv
RUN mkdir /opt/ElixirMessagingServer/priv/html5
RUN mkdir /opt/ElixirMessagingServer/priv/html5/resources
ADD Html5Client/src /opt/ElixirMessagingServer/priv/html5
ADD Html5Client/libs /opt/ElixirMessagingServer/priv/html5
ADD Html5Client/resources /opt/ElixirMessagingServer/priv/html5/resources

#RUN wget https://www.dropbox.com/sh/v4xel416t8mcbya/AABPc8LtNHq8c4oRBl-rBVona/ElixirServer.1.0.tgz?dl=0 \
# && tar -xvzf ElixirServer.1.0.tgz -C /opt \
# && rm ElixirServer.1.0.tgz

EXPOSE 80

CMD ["/usr/bin/iex"]
