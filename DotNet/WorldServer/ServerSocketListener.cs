using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Sean.World
{

	// State object for reading client data asynchronously
	public class ClientConnection {
        public ClientConnection(TcpClient inClientSocket)
        {
            this.socket = inClientSocket;
        }

        public static List<ClientConnection> clientsList = new List<ClientConnection> ();

	    public TcpClient socket = null;
	    public const int BufferSize = 1024;
	    public byte[] buffer = new byte[BufferSize];
	    public StringBuilder sb = new StringBuilder();  

        public void StartClient()
        {
            Thread ctThread = new Thread(DoConversation);
            ctThread.Start();
        }

        private void DoConversation()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            try {
                while (true) {
                    NetworkStream networkStream = socket.GetStream ();
                    int bytesRead = networkStream.Read (bytesFrom, 0, BufferSize);
                    if (bytesRead > 0)
                    {
                        Console.WriteLine ("Read {0} bytes", bytesRead);
                        {
                            var builder = new StringBuilder ();
                            for (int i=0; i<bytesFrom[0] + 1; i++) {
                                builder.Append (bytesFrom [i].ToString ());
                                builder.Append (",");
                            }
                            Console.WriteLine ("{0}", builder.ToString ());
                        }

                        var recv = MessageParser.ReadMessage (bytesFrom);

                        // Send a response back to the client.
                        var messageBuilder = new CommsMessages.Message.Builder ();
                        messageBuilder.SetMsgtype ((int)CommsMessages.MsgType.eResponse);
                        var respBuilder = new CommsMessages.Response.Builder ();
                        respBuilder.SetCode (0);
                        respBuilder.SetMessage ("OK");
                        messageBuilder.SetResponse (respBuilder);
                        var message = messageBuilder.BuildPartial ();

                        var messageBytes = MessageParser.WriteMessage (message);
                        Send (messageBytes);
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine (ex.ToString ());
            }
        }

        public void Send(byte[] byteData) {
            Console.WriteLine ("Send");
            var builder = new StringBuilder ();
            for (int i=0; i<byteData.Length; i++) {
                builder.Append (byteData [i].ToString ());
                builder.Append (",");
            }
            Console.WriteLine ("{0}", builder.ToString());

            // Begin sending the data to the remote device.
            NetworkStream socketStream = socket.GetStream();
            socketStream.Write(byteData, 0, byteData.Length);
            socketStream.Flush ();
        }
         
        /*
        public static void broadcast(byte[] byteData)
        {
            foreach (ClientConnection client in clientsList)
            {
                client.Send (byteData);
            }
        }
        */
	}

	public class ServerSocketListener {
		public static int ServerListenPort = 8084;
        private const int MaxMessageBufferSize = 1024;

	    // Thread signal.
	    public static ManualResetEvent allDone = new ManualResetEvent(false);

	    public ServerSocketListener() {
	    }

	    public static void Run() {
            Thread thread = new Thread (new ThreadStart (StartListening));
            thread.Start ();
        }
        private static void StartListening() {
            try {
                TcpListener serverSocket = new TcpListener(ServerListenPort);
                TcpClient clientSocket = default(TcpClient);
                serverSocket.Start();
                Console.WriteLine("Waiting for a connection...");
                while (true) {
                    var socket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client joined");
                    var client = new ClientConnection (socket);
                    ClientConnection.clientsList.Add(client);
                    client.StartClient();
                }
                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine("Ending Listening Server");
            }
            catch (Exception e) {
                Console.WriteLine("Exception caught in ServerSocketListener - {0}", e.ToString());
            }

            /*
	        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
	        IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerListenPort);
	        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
	        try {
	            listener.Bind(localEndPoint);
	            listener.Listen(100);
	            while (true) {
	                allDone.Reset();
	                Console.WriteLine("Waiting for a connection...");
	                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener );
                    allDone.WaitOne(); // Wait until a connection is made before continuing.
	            }
	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
            */   
	    }

        /*
	    public static void AcceptCallback(IAsyncResult ar) {
            Console.WriteLine ("AcceptCallback");
	        // Signal the main thread to continue.
	        allDone.Set();

	        // Get the socket that handles the client request.
	        Socket listener = (Socket) ar.AsyncState;
	        Socket handler = listener.EndAccept(ar);

	        // Create the state object.
			ClientConnection state = new ClientConnection();
	        state.socket = handler;
			handler.BeginReceive( state.buffer, 0, ClientConnection.BufferSize, 0,
	            new AsyncCallback(ReadCallback), state);
	    }
        */

        /*
	    public static void ReadCallback(IAsyncResult ar) {
            Console.WriteLine ("ReadCallback");
	        String content = String.Empty;

	        // Retrieve the state object and the handler socket
	        // from the asynchronous state object.
			ClientConnection state = (ClientConnection) ar.AsyncState;
	        Socket handler = state.socket;

	        // Read data from the client socket. 
	        int bytesRead = handler.EndReceive(ar);

	        if (bytesRead > 0) {
                Console.WriteLine ("Read {0} of {1} bytes", bytesRead, state.buffer[0] + 1);
                {
                    var builder = new StringBuilder ();
                    for (int i=0; i<state.buffer[0] + 1; i++) {
                        builder.Append (state.buffer [i].ToString ());
                        builder.Append (",");
                    }
                    Console.WriteLine ("{0}", builder.ToString ());
                }
	            // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.UTF8.GetString(state.buffer,0,bytesRead));

	            // Check for end-of-file tag. If it is not there, read more data.
	            content = state.sb.ToString();
                if (state.buffer[0] + 1 <= state.buffer.Length) {
	                // All the data has been read from the client.
	                Console.WriteLine("Read {0} bytes from socket", content.Length );

                    var recv = MessageParser.ReadMessage (state.buffer);

	                // Echo a response back to the client.
                    var messageBuilder = new CommsMessages.Message.Builder ();
                    messageBuilder.SetMsgtype((int)CommsMessages.MsgType.eResponse);
                    var respBuilder = new CommsMessages.Response.Builder ();
                    respBuilder.SetCode(0);
                    respBuilder.SetMessage("OK");
                    messageBuilder.SetResponse(respBuilder);
                    var message = messageBuilder.BuildPartial ();

                    var messageBytes = MessageParser.WriteMessage (message);
                    Send(handler, messageBytes);
	            } else {
	                // Not all data received. Get more.
					handler.BeginReceive(state.buffer, 0, ClientConnection.BufferSize, 0,
	                    new AsyncCallback(ReadCallback), state);
	            }
	        }
	    }

        private static void Send(Socket handler, byte[] byteData) {
            Console.WriteLine ("Send");
	        // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.UTF8.GetBytes(data);

            var builder = new StringBuilder ();
            for (int i=0; i<byteData.Length; i++) {
                builder.Append (byteData [i].ToString ());
                builder.Append (",");
            }
            Console.WriteLine ("{0}", builder.ToString());

	        // Begin sending the data to the remote device.
	        handler.BeginSend(byteData, 0, byteData.Length, 0,
	            new AsyncCallback(SendCallback), handler);
	    }

	    private static void SendCallback(IAsyncResult ar) {
	        try {
                Console.WriteLine ("SendCallback");
	            // Retrieve the socket from the state object.
	            Socket handler = (Socket) ar.AsyncState;

	            // Complete sending the data to the remote device.
	            int bytesSent = handler.EndSend(ar);
	            Console.WriteLine("Sent {0} bytes to client.", bytesSent);
	            handler.Shutdown(SocketShutdown.Both);
	            handler.Close();

	        } catch (Exception e) {
	            Console.WriteLine(e.ToString());
	        }
	    }
        */   
	}

}