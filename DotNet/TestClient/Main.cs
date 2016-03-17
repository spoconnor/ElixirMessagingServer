using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestClient
{
    static class MainClass
    {
        static public void Main (String [] args)
        {
            var result = Test ();
        }

        static private async Task<HttpResponseMessage> Test()
        {
            try {
                var client = new HttpClient ();
                var response = await client.GetAsync ("http://localhost:9000/wibble");
                response.EnsureSuccessStatusCode();
            } 
            catch (Exception e) 
            {
                Console.WriteLine ($"Exception caught - {e.Message}");
                return new HttpResponseMessage (System.Net.HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage (System.Net.HttpStatusCode.OK);
        }
    }
}
