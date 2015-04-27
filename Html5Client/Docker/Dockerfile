FROM ubuntu:14.04
RUN apt-get update && apt-get upgrade -y
RUN apt-get install -y openssh-server apache2 supervisor
RUN mkdir -p /var/lock/apache2 /var/run/apache2 /var/run/sshd /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
COPY index.html /var/www/html/index.html
COPY phaser.min.js /var/www/html/phaser.min.js
COPY phaser.png /var/www/html/phaser.png
EXPOSE 22 80
CMD ["/usr/bin/supervisord"]

