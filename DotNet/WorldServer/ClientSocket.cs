using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sean.World
{
    public static class ClientSocket
    {
        private static SynchronousSocketClient socket;
        private const string server = "elixirserver";
        private const int port = 8083;

        private static SynchronousSocketClient GetSocket()
        {
            if (socket == null) {
                socket = new SynchronousSocketClient ("elixirserver", 8083);
            }
            return socket;
        }

        public static void SendMessage()
        {
            // TODO - threads
            GetSocket ().SendMessage ();
        }

        public static void RecvMessage()
        {
            // TODO - threads
            GetSocket ().RecvMessage ();
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
                    socket = null;
                }
            }
            // Free any unmanaged objects here.
            disposed = true;
        }

        private Socket socket;
        private int port;
        private static IPHostEntry remoteHost;
        private IPAddress ipAddress;

        private bool IsConnected
        {
            get
            {
                return socket != null && socket.IsBound;
            }
        }

        private void Connect() 
        {
            Console.WriteLine("SynchronousSocketClient.StartClient");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            socket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp );

            socket.Connect(remoteEP);
            Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());
        }

        public void SendMessage()
        {
            try {
                if (!IsConnected) Connect();
                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes ("This is a test<EOF>");
                int bytesSent = socket.Send (msg);
                Console.WriteLine("SynchronousSocketClient.SendMessage sent {0} bytes", bytesSent);
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
                if (!IsConnected) Connect();
                // Receive the response from the remote device.
                byte[] bytes = new byte[1024];
                int bytesRec = socket.Receive (bytes);
                Console.WriteLine("SynchronousSocketClient.RecvMessage received {0} bytes", bytesRec);
             } catch (SocketException se) {
                Console.WriteLine ("SocketException : {0}", se.ToString ());
            } catch (Exception ex) {
                Console.WriteLine ("Unexpected exception : {0}", ex.ToString ());
            }
        }
    }
}
