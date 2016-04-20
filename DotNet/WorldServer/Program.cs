using System;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace Sean.World
{
    static class MainClass
	{
        static WorldMapData worldMapData;

		static public void Main(String[] args)
		{
			Console.WriteLine("World Server...");

            /*
            // Start OWIN host 
            string baseAddress = "http://localhost:9000/"; 
            using (WebApp.Start<Startup>(url: baseAddress)) 
            { 
                HttpClient client = new HttpClient(); 
                var response = client.GetAsync(baseAddress + "api/Test").Result; // Test call
                Console.WriteLine(response); 
                Console.WriteLine(response.Content.ReadAsStringAsync().Result); 
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            */

            var size = new ArraySize(){minX=0, maxX=60, minZ=0, maxZ=20, scale=1, period=20};
            var data = PerlinNoise.GetIntMap(size, 1);
            data.Render();
            Console.WriteLine ();
            size = new ArraySize(){minX=0, maxX=60, minZ=20, maxZ=40, scale=1, period=20};
            data = PerlinNoise.GetIntMap(size, 1);
            data.Render();

            //var d = new Array<int> (new ArraySize(){minX=50, maxX=100, minZ=50, maxZ=100, scale=5});
            //foreach (var a in d.GetLines()) {
            //
            //}

            worldMapData = new WorldMapData();
            worldMapData.Generate();

            while (true)
            {
                Console.Clear ();
                worldMapData.Render ();
                var key = Console.ReadKey ();
                switch (key.KeyChar)
                {
                    case 'q': Cursor.MoveUp (); break;
                    case 'a': Cursor.MoveDown (); break;
                    case 'p': Cursor.MoveRight (); break;
                    case 'o': Cursor.MoveLeft (); break;
                    case '+': Cursor.ZoomIn (); break;
                    case '-': Cursor.ZoomOut (); break;
                }
            }

			WorldData.Initialize(); // TODO - not hooked in

            //var otpServer = new OtpServer();
            //otpServer.Start();

            //ClientSocket.SendMessage ();

            ServerSocketListener.Run();

            while (true) {
                var mapData = worldMapData.Serialize().ToArray();

                var messageBuilder = new CommsMessages.Message.Builder ();
                messageBuilder.SetMsgtype ((int)CommsMessages.MsgType.eMap);
                messageBuilder.SetDest (0);
                messageBuilder.SetFrom (1);
                var mapBuilder = new CommsMessages.Map.Builder ();
                mapBuilder.SetMinX (0);
                mapBuilder.SetMinY (0);
                mapBuilder.SetMaxX (50);
                mapBuilder.SetMaxY (80);
                mapBuilder.SetDataSize (mapData.Length);
                messageBuilder.SetMap (mapBuilder);
                var message = messageBuilder.BuildPartial ();
                //var messageBytes = MessageParser.WriteMessage (message);

                //ClientConnection.broadcast(messageBytes.Concat(mapData).ToArray());
//                ClientSocket.SendMessage (message, mapData);
                
                System.Threading.Thread.Sleep (10000);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}
	}
}
