using System;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace Sean.World
{
    static class MainClass
	{
        static WorldMap worldMapData;

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


            var size = new ArraySize(){
                minX=0, maxX=40, minZ=0, maxZ=40, 
                scale=1, maxY=10};
            var data = PerlinNoise.GetIntMap(size,1);
            data.Render();
            Console.WriteLine ();
            size = new ArraySize(){
                minX=0, maxX=60, minZ=30, maxZ=60, 
                scale=1, maxY=10};
            data = PerlinNoise.GetIntMap(size,3);
            data.Render();

            //var d = new Array<int> (new ArraySize(){minX=50, maxX=100, minZ=50, maxZ=100, scale=5});
            //foreach (var a in d.GetLines()) {
            //
            //}


            worldMapData = new WorldMap();
            worldMapData.Generate();
            /*
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
            */

            //var otpServer = new OtpServer();
            //otpServer.Start();

            //ClientSocket.SendMessage ();

            ServerSocketListener.Run();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}
	}
}
