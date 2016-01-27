using Owin; 
using System.Web.Http; 

namespace Sean.World 
{ 
    public class Startup 
    { 
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder) 
        { 
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration(); 
            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}", 
                defaults: new { id = RouteParameter.Optional } 
            ); 
            //config
            //    .EnableSwagger(c => c.SingleApiVersion("v1", "World Server Rest API"))
            //    .EnableSwaggerUi();

            appBuilder.UseWebApi(config); 
        } 
    } 
} 

