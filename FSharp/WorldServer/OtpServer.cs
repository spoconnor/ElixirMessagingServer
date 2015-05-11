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

			String host = System.Net.Dns.GetHostName();
			//String user = Environment.UserName;
            const String name = "world";
			const String cookie = "cookie";
			node = new OtpNode(name + "@" + host, true, cookie);
			mbox = null;

            Console.WriteLine("This node is: {0} (cookie='{1}')", node.node(), node.cookie());

            const String remote = "client@zen";
            if (node.ping(remote, 1000 * 300) != true)
            {
                Console.WriteLine("Failed to Ping remote at {0}", remote);
                return;
            }

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

			try
			{
				//if (conn != null)
				//	System.Console.Out.WriteLine("   successfully pinged node " + remote + "\n");
				//else
				//	throw new System.Exception("Could not ping node: " + remote);

				//conn.traceLevel = 1;

				mbox = node.createMbox();
                if (mbox.registerName("server") != true)
                {
                    Console.WriteLine("Failed to register name");
                    return;
                }

                while (true)
                {
                    Otp.Erlang.Object msg = mbox.receive();
                    Console.WriteLine("IN msg: " + msg.ToString() + "\n");
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
