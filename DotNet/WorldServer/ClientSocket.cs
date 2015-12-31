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

    public class SynchronousSocketClient : IDisposable
    {
        public SynchronousSocketClient(string server, int serverPort)
        {
            remoteHost = Dns.GetHostEntry(server);
            ipAddress = remoteHost.AddressList[0];
            port = serverPort;
        }

        // Dispose pattern
        bool disposed = false;
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);           
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 
            if (disposing) {
                // Free any other managed objects here.
                if (socket != null) {
                    socket.Shutdown (SocketShutdown.Both);
                    socket.Close ();
                }
            }
            // Free any unmanaged objects here.
            disposed = true;
        }

        private Socket socket;
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
                socket = new Socket(
                    AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );

                socket.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());
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

        public void SendMessage()
        {
            try {
                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes ("This is a test<EOF>");
                int bytesSent = socket.Send (msg);
            } catch (ArgumentNullException ane) {
                Console.WriteLine ("ArgumentNullException : {0}", ane.ToString ());
            } catch (SocketException se) {
                Console.WriteLine ("SocketException : {0}", se.ToString ());
            } catch (Exception ex) {
                Console.WriteLine ("Unexpected exception : {0}", ex.ToString ());
            }
        }

        public void RecvMessage()
        {
            try {
                // Receive the response from the remote device.
                int bytesRec = socket.Receive (bytes);
                Console.WriteLine ("Echoed test = {0}", Encoding.ASCII.GetString (bytes, 0, bytesRec));
            } catch (ArgumentNullException ane) {
                Console.WriteLine ("ArgumentNullException : {0}", ane.ToString ());
            } catch (SocketException se) {
                Console.WriteLine ("SocketException : {0}", se.ToString ());
            } catch (Exception ex) {
                Console.WriteLine ("Unexpected exception : {0}", ex.ToString ());
            }
        }
    }