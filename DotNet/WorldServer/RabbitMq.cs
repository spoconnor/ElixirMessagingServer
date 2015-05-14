using System;
using RabbitMQ.Client;

namespace Sean.World
{
    public class RabbitMq
    {
        public RabbitMq()
        {
        }

        static public void Publish()
        {
            ConnectionFactory factory = new ConnectionFactory();

            // The next six lines are optional:
            factory.UserName = ConnectionFactory.DefaultUser;
            factory.Password = ConnectionFactory.DefaultPass;
            factory.VirtualHost = ConnectionFactory.DefaultVHost;
            factory.HostName = "localhost";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;

            // You also could do this instead:
            factory.Uri = "amqp://localhost";

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            channel.QueueDeclare("hello-world-queue", // queue
                false, // durable
                false, // exclusive
                false, // autoDelete
                null); // arguments

            byte[] message = Encoding.UTF8.GetBytes("Hello, World!");
            channel.BasicPublish(string.Empty, // exchange
                "hello-world-queue", // routingKey
                null, // basicProperties
                message); // body

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            channel.Close();
            connection.Close();
        }

        static public void Subscribe()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.QueueDeclare("hello-world-queue", false, false, false, null);

            // BasicGet: Retrieve an individual message, if one is available.
            // Returns null if the server answers that no messages are currently available. 
            BasicGetResult result = channel.BasicGet("hello-world-queue", // queue
                                true); // noAck (true=auto ack, false=we must call BasicAck ourselves)

            if (result != null)
            {
                string message = Encoding.UTF8.GetString(result.Body);
                Console.WriteLine(message);

                // If the noAck parameter to BasicGet was false then:
                // channel.BasicAck(result.DeliveryTag // deliveryTag
                //  false); // multiple (not sure what this means)
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            channel.Close();
            connection.Close();
        }
    }
}

