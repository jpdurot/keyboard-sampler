using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Sampler.Server.Utils;

namespace Sampler
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Activate SignalR for websocket with web app
            appBuilder.MapSignalR("/socket", new HubConfiguration());
            
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
			
			appBuilder.Use(async (context, next) =>
			{
				// Add Header
				context.Response.Headers["Access-Control-Allow-Origin"] = "localhost:9000, samplairre.progx.org:9000";
				
				if (context.Request.Method == "OPTIONS"){

					context.Response.StatusCode = 200;

					context.Response.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Methods", "GET", "POST", "PUT", "DELETE");

					context.Response.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Headers", "authorization", "content-type");
					return;

				}

				// Call next middleware
				await next();
			});
            
            appBuilder.UseWebApi(config);

            var fileSystem = new PhysicalFileSystem("www");

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem,
            };
            options.StaticFileOptions.ContentTypeProvider = new CustomContentTypeProvider();

            appBuilder.UseFileServer(options);

            config.EnsureInitialized();
        }
    }
}
