using System;
using Otp;

namespace Sean.World
{

	// Start server with
	// iex --sname servernode --cookie cookie mathserver.ex                             

	class MainClass
	{
		static public void Main(String[] args)
		{
			System.Console.Out.WriteLine("World Server...");
            WorldMapData worldMapData = new WorldMapData();
            worldMapData.Generate();

			WorldData.Initialize();


			if (args.Length < 1)
			{
				System.Console.Out.WriteLine("Usage: Otp sname\n  where sname is the short name of the Erlang node");
				return;
			}

			OtpNode.useShortNames = true;

			String  host   = System.Net.Dns.GetHostName();
			String  user   = Environment.UserName;
			String  cookie = "cookie";
			OtpNode node   = new OtpNode(user + "@" + host, false, cookie);
			String  remote = (args[0].Contains("@")) ? args[0] : args[0] + "@" + host;
			OtpMbox mbox   = null;

			System.Console.Out.WriteLine("This node is: {0} (cookie='{1}'). Remote: {2}",
				node.node(), node.cookie(), remote);

			//bool ok = node.ping(remote, 1000*300);

			OtpCookedConnection conn = node.connection(remote);

			try
			{
				if (conn != null)
					System.Console.Out.WriteLine("   successfully pinged node " + remote + "\n");
				else
					throw new System.Exception("Could not ping node: " + remote);

				//conn.traceLevel = 1;

				mbox = node.createMbox();
				mbox.registerName("server");

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

				while (true)
				{
					Otp.Erlang.Object msg = mbox.receive();
					System.Console.Out.WriteLine("IN msg: " + msg.ToString() + "\n");
				}
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
