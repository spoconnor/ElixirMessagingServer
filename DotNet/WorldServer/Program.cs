using System;

namespace Sean.World
{
    static class MainClass
	{
        static WorldMapData worldMapData;

		static public void Main(String[] args)
		{
			Console.WriteLine("World Server...");
            worldMapData = new WorldMapData();
            worldMapData.Generate();

			WorldData.Initialize(); // TODO - not hooked in

            //var otpServer = new OtpServer();
            //otpServer.Start();

            //ClientSocket.SendMessage ();

            ServerSocketListener.StartListening();


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}
	}
}
