# My Docker file
# To build:
# sudo docker build -t my_elixir_server .

FROM ubuntu
MAINTAINER sean

# Erlang
ADD http://packages.erlang-solutions.com/ubuntu/erlang_solutions.asc /usr/local/src/erlang_solutions.asc
RUN apt-key add /usr/local/src/erlang_solutions.asc
RUN apt-get update
RUN apt-get install erlang -y

# Git, Unzip
RUN apt-get install git -y
RUN apt-get install unzip -y

# Elixir
ADD https://github.com/elixir-lang/elixir/releases/download/v1.0.0/Precompiled.zip /usr/local/src/Precompiled.zip
RUN cd /usr/local/src
RUN unzip /usr/local/src/Precompiled.zip -d /usr

# Riak
ADD http://s3.amazonaws.com/downloads.basho.com/riak/2.0/2.0.5/ubuntu/trusty/riak_2.0.5-1_amd64.deb /usr/local/src/
RUN dpkg -i /usr/local/src/riak_2.0.5-1_amd64.deb

CMD "echo" "Hello World!"
