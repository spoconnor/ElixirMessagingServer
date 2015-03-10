rabbitmq-server -detached -setcookie 'SeansSecretRabbitMqCookie' 
sleep 10 
rabbitmqctl start_app 
sleep 10 
/var/MyApp/rabbitmqadmin import /var/MyApp/rabbit_queue_defs.json
/var/MyApp/WorldServer &
/var/MyApp/ElixirMessagingServer
