using System;
using Otp;

namespace Sean.World
{
	class MainClass
	{
		static public void Main(String[] args)
		{
			Console.WriteLine("World Server...");
            WorldMapData worldMapData = new WorldMapData();
            worldMapData.Generate();

			WorldData.Initialize();

            //var otpServer = new OtpServer();
            //otpServer.Start();

            ServerSocketListener.StartListening();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}
	}
}
