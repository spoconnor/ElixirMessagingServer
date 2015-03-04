# My Docker file
# To build:
# sudo docker build -t my_elixir_server .

FROM ubuntu
MAINTAINER sean

# Erlang
ADD http://packages.erlang-solutions.com/ubuntu/erlang_solutions.asc /tmp/install/
ADD http://packages.erlang-solutions.com/erlang-solutions_1.0_all.deb /tmp/install/
RUN apt-key add /tmp/install/erlang_solutions.asc
RUN dpkg -i /tmp/install/erlang-solutions_1.0_all.deb
RUN apt-get update
RUN apt-get install erlang -y

# Git, Unzip
RUN apt-get install git -y
RUN apt-get install unzip -y

# Elixir
ADD https://github.com/elixir-lang/elixir/releases/download/v1.0.0/Precompiled.zip /tmp/install/
RUN unzip /tmp/install/Precompiled.zip -d /usr

# Riak
ADD http://s3.amazonaws.com/downloads.basho.com/riak/2.0/2.0.5/ubuntu/trusty/riak_2.0.5-1_amd64.deb /tmp/install/
RUN dpkg -i /tmp/install/riak_2.0.5-1_amd64.deb

CMD "echo" "Hello World!"
