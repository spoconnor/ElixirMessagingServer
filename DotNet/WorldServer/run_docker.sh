sudo docker run --name worldserver -i -t -v /mnt/archive/Programming/Cloud/ElixirMessagingServer/DotNet/WorldServer:/opt/WorldServer mono:4.2.1.102 bash
#sudo docker build -t my_elixir_server .
#sudo docker run --name elixirserver -i -t my_elixir_server /opt/ElixirMessagingServer/ElixirMessagingServer
sudo docker rm worldserver
