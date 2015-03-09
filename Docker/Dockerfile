# My Docker file
# To build:
# sudo docker build -t my_elixir_server .

FROM ubuntu:14.04
#FROM ubuntu:12.04
#FROM ubuntu
MAINTAINER sean

# Erlang
ADD http://packages.erlang-solutions.com/ubuntu/erlang_solutions.asc /tmp/install/
ADD http://packages.erlang-solutions.com/erlang-solutions_1.0_all.deb /tmp/install/
RUN apt-key add /tmp/install/erlang_solutions.asc
RUN dpkg -i /tmp/install/erlang-solutions_1.0_all.deb
RUN apt-get update
RUN apt-get install erlang -y

# Various utils
RUN apt-get install -y wget
RUN apt-get install -y git 
RUN apt-get install -y unzip 
RUN apt-get install -y libprotoc-dev 
RUN apt-get install -y rabbitmq-server
RUN apt-get install -y librabbitmq1
RUN apt-get install -y libboost-chrono1.54.0

# Riak
#ADD http://s3.amazonaws.com/downloads.basho.com/riak/2.0/2.0.5/ubuntu/trusty/riak_2.0.5-1_amd64.deb /tmp/install/
#RUN dpkg -i /tmp/install/riak_2.0.5-1_amd64.deb

RUN rabbitmq-plugins enable rabbitmq_management
RUN service rabbitmq-server start

# My code
RUN wget -P /tmp/install https://s3-ap-southeast-2.amazonaws.com/onewheel/installation/MyApp.20150309-2243.tgz
RUN tar -xvzf /tmp/install/MyApp.*.tgz -C /var
RUN mv /var/MyApp/lib* /usr/lib
RUN chmod +x /var/MyApp/ElixirMessagingServer
RUN chmod +x /var/MyApp/WorldServer
RUN chmod +x /var/MyApp/*.sh
RUN ln -s /usr/lib/libSimpleAmqpClient.so.2.4.0 /usr/lib/libSimpleAmqpClient.so.2
RUN ln -s /usr/lib/libSimpleAmqpClient.so.2.4.0 /usr/lib/libSimpleAmqpClient.so

# Elixir - For demo
ADD https://github.com/elixir-lang/elixir/releases/download/v1.0.0/Precompiled.zip /tmp/install/
RUN unzip /tmp/install/Precompiled.zip -d /usr

WORKDIR /var/MyApp

# Web port
EXPOSE 80

# RabbitMq port
#EXPOSE 5672 15672

CMD ["/var/MyApp/start_all.sh"]
