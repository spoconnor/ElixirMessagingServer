FROM ubuntu:14.04
RUN apt-get update && apt-get upgrade -y
RUN apt-get install -y openssh-server apache2 supervisor
RUN mkdir -p /var/lock/apache2 /var/run/apache2 /var/run/sshd /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
COPY libs /var/www/html/
COPY resources /var/www/html/resources/
COPY src /var/www/html/
EXPOSE 22 80
CMD ["/usr/bin/supervisord"]

