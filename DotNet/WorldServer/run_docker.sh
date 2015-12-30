#sudo docker network create overlay
sudo docker run --name worldserver -h worldserver --net=overlay -p 8084:8084 -i -t -v /mnt/archive/Programming/Cloud/ElixirMessagingServer/DotNet/WorldServer:/opt/WorldServer mono:4.2.1.102 bash
#sudo docker build -t my_elixir_server .
#sudo docker run --name elixirserver -i -t my_elixir_server /opt/ElixirMessagingServer/ElixirMessagingServer
sudo docker rm worldserver
