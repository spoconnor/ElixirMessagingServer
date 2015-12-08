sudo docker run --name consul -h consul  \
    -p 8300:8300 \
    -p 8301:8301 \
    -p 8301:8301/udp \
    -p 8302:8302 \
    -p 8302:8302/udp \
    -p 8400:8400 \
    -p 8500:8500 \
    -p 10.1.1.4:53:53 \
    -d -v /mnt:/data \
    progrium/consul -server 

