using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otp;

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
            if (node != null)
            {
	            node.closeMbox(mbox);
			    node.close();
                node = null;
            }
        }
        #endregion

        private OtpNode node;
        private OtpMbox mbox;

        public void Execute()
        {
			System.Console.Out.WriteLine("Starting Otp Server...");

			OtpNode.useShortNames = true;

			String  host   = System.Net.Dns.GetHostName();
			//String  user   = Environment.UserName;
            String name = "world";
			String  cookie = "cookie";
			node   = new OtpNode(name + "@" + host, false, cookie);
			//String  remote = (args[0].Contains("@")) ? args[0] : args[0] + "@" + host;
			mbox   = null;

			System.Console.Out.WriteLine("This node is: {0} (cookie='{1}')",
				node.node(), node.cookie());

			//bool ok = node.ping(remote, 1000*300);

			//OtpCookedConnection conn = node.connection(remote);

			try
			{
				//if (conn != null)
				//	System.Console.Out.WriteLine("   successfully pinged node " + remote + "\n");
				//else
				//	throw new System.Exception("Could not ping node: " + remote);

				//conn.traceLevel = 1;

				mbox = node.createMbox();
                mbox.registerName(name + "@" + host); //"server");

                while (true)
                {
                    Otp.Erlang.Object msg = mbox.receive();
                    System.Console.Out.WriteLine("IN msg: " + msg.ToString() + "\n");
                }

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
			finally
			{
				node.closeMbox(mbox);
			}

			node.close();
		}


    }
}
