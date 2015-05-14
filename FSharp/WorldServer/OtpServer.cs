using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otp;
using NFX.Erlang;

namespace Sean.World
{
    // Start server with
    // iex --sname servernode --cookie cookie mathserver.ex                             

    class OtpServer
    {
        public OtpServer()
        {
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var otpServer = new OtpServerInstance();
                otpServer.Execute();
            });
        }
    }

    class OtpServerInstance : IDisposable
    {
        public void OtpServer()
        {
        }

        ~OtpServerInstance()
        {
            Dispose(false);
        }

        #region IDisposable Members
        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            this.IsDisposed = true;
			if (self != null)
            {
				self.closeMbox(mbox);
				//self.unPublishPort();
				self = null;
            }
        }
        #endregion

		private OtpNode self;
        private OtpMbox mbox;
        private const String name = "world";
        private const String cookie = "cookie";
        private const String mailboxname = "server";

        public void Execute()
        {
			try
			{
            	System.Console.Out.WriteLine("Starting Otp Server...");


				var mbox = ErlApp.Node.CreateMbox("test");

				while (App.Active)
				{
					var result = mbox.Receive(1000);
					if (result != null)
						Console.WriteLine("Mailbox {0} got message: {1}", mbox.Self, result);
				}


            	OtpNode.useShortNames = true;
            	String host = System.Net.Dns.GetHostName();
            	//String user = Environment.UserName;
            	//String remote = "client@" + host;


				self = new OtpNode(name + "@" + host, true, cookie);
				//node = new OtpSelf(name + "@" + host, cookie);
				//node.publishPort();
				//OtpPeer other = new OtpPeer("server");
				//OtpConnection conn = self.connect(other); // connect to peer

				Console.WriteLine("This node is: {0}, cookie='{1}'", self.node(), self.cookie());

				mbox = self.createMbox("mbox");
				Otp.Erlang.Object[] tupleElements = new Otp.Erlang.Object[2];

				Otp.Erlang.Object[] selfTuple = new Otp.Erlang.Object[2];
				selfTuple[0] = new Otp.Erlang.Atom("mbox");
				selfTuple[1] = new Otp.Erlang.Atom(self.node());

				OtpEpmd.publishPort(self);
				tupleElements[0] = new Otp.Erlang.Tuple(selfTuple);

				Console.WriteLine("Listening...");
				while (true) try {
					Otp.Erlang.Tuple terms = (Otp.Erlang.Tuple) mbox.receive();
					Otp.Erlang.Pid from = (Otp.Erlang.Pid) terms.elementAt(0);
					Otp.Erlang.Tuple msg = (Otp.Erlang.Tuple) terms.elementAt(1);

					String command = msg.elementAt(0).ToString();
					Console.WriteLine("Received {0}", command);

					if (command == "echo") {
						mbox.send(from, new Otp.Erlang.Atom(msg.elementAt(1).ToString()));
					}
					else if (command == "add") {
						long x = ((Otp.Erlang.Long) msg.elementAt(1)).longValue();
						long y = ((Otp.Erlang.Long) msg.elementAt(2)).longValue();

						mbox.send(from, new Otp.Erlang.Long(x+y));
					}
					else if (command == "add2") {
						long x = ((Otp.Erlang.Long) msg.elementAt(1)).longValue();
						long y = ((Otp.Erlang.Long) msg.elementAt(2)).longValue();
						tupleElements[1] = new Otp.Erlang.Long(x+y);

						mbox.send(from, new Otp.Erlang.Tuple(tupleElements));
					}
				}
				catch (Otp.Erlang.Exit e) {
					break;
				}


            	//Console.WriteLine("Pinging {0}", remote);
            	//if (node.ping(remote, 1000 * 300) != true)
            	//{
            	//    Console.WriteLine("Failed to Ping remote at {0}", remote);
            	//    return;
            	//}

            	//OtpCookedConnection conn = node.connection(remote);
	
    	        //OtpSelf cNode = new OtpSelf("clientnode", "cookie");
            	//OtpPeer sNode = new OtpPeer("servernode@chloe.ravnaandtines.com");
            	//OtpConnection connection = cNode.connect(sNode);
            	//
            	//OtpErlangObject[] args = new OtpErlangObject[]{new OtpErlangLong(1), new OtpErlangLong(2)};
            	//connection.sendRPC("mathserver", "add", args);
            	//OtpErlangLong sum = (OtpErlangLong) connection.receiveRPC();
	            //if (sum.intValue() != 3)
            	//{
            	//    throw new System.SystemException("Assertion failed, returned = " + sum.intValue());
            	//}
            	//System.Console.Out.WriteLine("OK!");

                //if (conn != null)
                //    System.Console.Out.WriteLine("   successfully pinged node " + remote + "\n");
                //else
                //    throw new System.Exception("Could not ping node: " + remote);
                //conn.traceLevel = 1;
			

                //mbox = node.createMbox();
                //Console.WriteLine("Registering mailbox {0}", mailboxname );
                //if (mbox.registerName(mailboxname) != true)
                //{
                //    Console.WriteLine("Failed to register name");
                //    return;
                //}

                /*
                Console.WriteLine("Waiting for connection...");
				var client = node.accept();
				Console.WriteLine("Accepted connection...");
                while (true)
                {
					Otp.Erlang.Object msg = client.receive();
                    Console.WriteLine("IN msg: " + msg.ToString() + "\n");
                }
				*/

                /*
                mbox.sendRPC(conn.peer.node(), "lists", "reverse", new Otp.Erlang.List(new Otp.Erlang.String("Hello world!")));
                //mbox.sendRPC(conn.peer.node(), "Elixir.Mathserver", "start_link", new Otp.Erlang.List());
                Otp.Erlang.Object reply = mbox.receiveRPC(5000);
                System.Console.Out.WriteLine("<= " + reply.ToString());

                {
                    Otp.Erlang.List rpcArgs = new Otp.Erlang.List(
                        new Otp.Erlang.Object[] {
                            mbox.self(),
                            new Otp.Erlang.Tuple(
                                new Otp.Erlang.Object[] {
                                    new Otp.Erlang.Atom("table"), new Otp.Erlang.Atom("test"), new Otp.Erlang.Atom("simple")
                                }
                            )
                        }
                    );

                    mbox.sendRPC(conn.peer.node(), "mnesia_subscr", "subscribe", rpcArgs);
                    reply = mbox.receiveRPC(5000);
                    System.Console.Out.WriteLine("<= " + reply.ToString());
                }
                */
            }
            catch (System.Exception e)
            {
                System.Console.Out.WriteLine("Error: " + e.ToString());
            }


            
        }
    }
}
