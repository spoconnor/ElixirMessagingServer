#include <SimpleAmqpClient.h>
#include <iostream>
#include <sys/time.h>
#include <stdlib.h>
#include "CommsMessages.pb.h"
#include "MapServer.h"
#include <boost/lexical_cast.hpp>

using namespace AmqpClient;


#ifdef _WIN32
  #include <windows.h>

  void msleep(unsigned milliseconds)
  {
    Sleep(milliseconds);
  }
#else
  #include <unistd.h>

  void msleep(unsigned milliseconds)
  {
    usleep(milliseconds * 1000); // takes microseconds
  }
#endif

int main()
{
  // Verify that the version of the library that we linked against is
  // compatible with the version of the headers we compiled against.
  GOOGLE_PROTOBUF_VERIFY_VERSION;

  const std::string HOSTNAME = "localhost";
  const int PORT = 5672;
  const std::string USERNAME = "guest";
  const std::string PASSWORD = "guest";
  const std::string VHOST = "/";
  const std::string EXCHANGE_NAME = "MyExchange";

  const std::string OUTBOUND_ROUTING_KEY = "Outbound";
  const std::string CONSUMER_TAG = "ConsumerTag";

  const std::string INBOUND_QUEUE_NAME = "InboundQueue";
  const std::string OUTBOUND_QUEUE_NAME = "OutboundQueue";
  //const std::string ERROR_QUEUE_NAME = "ErrorQueue";


  try
  {
    Channel::ptr_t channel = Channel::Create(HOSTNAME, PORT, USERNAME, PASSWORD, VHOST);
    channel->BasicConsume(INBOUND_QUEUE_NAME, CONSUMER_TAG, true, true, false);

    Envelope::ptr_t env;
    while (true)
    {
      if (channel->BasicConsumeMessage(CONSUMER_TAG, env, 0))
      {
        std::string bodyStr = env->Message()->Body().substr(2, -1);
        std::string headStr = env->Message()->Body().substr(0, 2);

        std::cout << "Envelope received: \n"
                  << " Exchange: " << env->Exchange()
                  << "\n Routing key: " << env->RoutingKey()
                  << "\n Consumer tag: " << env->ConsumerTag()
                  << "\n Delivery tag: " << env->DeliveryTag()
                  << "\n Redelivered: " << env->Redelivered()
                  << "\n Body: " << bodyStr << std::endl;

        Header header;
        header.ParseFromString(headStr);

        Ping pingMsg;
        Pong pongMsg;
        Response responseMsg;
        NewUser newUserMsg;
        Login loginMsg;
        Say sayMsg;
        //Map mapMsg;
        //TArray2<uint>* map;
        std::string replyBodyStr = "";
        switch (header.msgtype())
        {
          case 1 : // Ping
            pongMsg.set_count(pingMsg.count());
            replyBodyStr = pongMsg.SerializeAsString();
            break;
          case 2: // Pong
            break;
          case 3: // Response
            break;
          case 4: // NewUser
            break;
          case 5: // Login
            if (!loginMsg.ParseFromString(bodyStr))
            {
              std::cout << "Error parsing 'Login' message.\n";
              continue;
            }
            std::cout << "Login '" << loginMsg.username() << std::endl;
            break;
          case 6: // Say
            if (!sayMsg.ParseFromString(bodyStr))
            {
              std::cout << "Error parsing 'Say' message.\n";
              continue;
            }
            std::cout << "Say: '" << sayMsg.text() << "'\n";
            replyBodyStr = bodyStr; // Bounce back to clients
            break;

          //case 8: // EnterMap
          //  if (!enterMapMsg.ParseFromString(bodyStr))
          //  {
          //    std::cout << "Error parsing 'EnterMap' message.\n";
          //    continue;
          //  }
          //  map = MapServer::GetMap(enterMapMsg.mapcoords().x(), enterMapMsg.mapcoords().y());
            //    mapMsg.set_allocated_mapcoords(enterMapMsg.mapcoords());
            //    mapMsg.mapsize().set_x(map->cols());
            //    mapMsg.mapsize().set_y(map->rows());

          //  struct timeval detail_time;
          //  gettimeofday(&detail_time,NULL);
          //  mapMsg.set_timestamp(detail_time.tv_usec); /* microseconds */
            //    mapMsg.set_data(map->c_data());
          //  replyBodyStr = mapMsg.SerializeAsString();
          //  break;

          default:
            std::cout << "Unknown message type " << header.msgtype() << std::endl;
        }

        msleep(2000);

        BasicMessage::ptr_t outgoing_message = BasicMessage::Create();
        outgoing_message->Body(replyBodyStr);
        channel->BasicPublish(EXCHANGE_NAME, OUTBOUND_ROUTING_KEY, outgoing_message);
      }
      else
      {
        std::cout << "Basic Consume failed.\n";
      }
      msleep(4000);
    }

  }
  catch (MessageReturnedException &e)
  {
    std::cout << "Message got returned: " << e.what();
    std::cout << "\nMessage body: " << e.message()->Body();
  }
  catch (AmqpException &e)
  {
    std::cout << "Failure: " << e.what();
  }

  // Optional:  Delete all global objects allocated by libprotobuf.
  google::protobuf::ShutdownProtobufLibrary();
}

