using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sean.World
{
    public class ClientSocket
    {
        public void Run()
        {
            try 
            {
                var client = new SynchronousSocketClient ("elixirserver", 8083);
                Thread oThread = new Thread(new ThreadStart(client.StartClient));
                oThread.Start();
                oThread.Join();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception creating ClientSocker thread : {0}", ex.ToString());
            }
        }
    }

    public class SynchronousSocketClient {

        public SynchronousSocketClient(string server, int serverPort)
        {
            remoteHost = Dns.GetHostEntry(server);
            ipAddress = remoteHost.AddressList[0];
            port = serverPort;
        }

        private int port;
        private static IPHostEntry remoteHost;
        private IPAddress ipAddress;

        public void StartClient() 
        {
            try 
            {
                Console.WriteLine("SynchronousSocketClient.StartClient");
                byte[] bytes = new byte[1024];

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket sender = new Socket(
                    AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );

                sender.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes,0,bytesRec));

                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            } 
            catch (ArgumentNullException ane) 
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            } 
            catch (SocketException se) 
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            } 
            catch (Exception ex) 
            {
                Console.WriteLine("Unexpected exception : {0}", ex.ToString());
            }
        }
    }
}