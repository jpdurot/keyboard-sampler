#if !DOTNETCORE
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.Cors;
using Owin;

using Microsoft.AspNet.SignalR;

using Sampler.Server.Utils;
using System;

namespace Sampler
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.

        public void Configuration(IAppBuilder appBuilder)
        {
            // Activate SignalR for websocket with web app
            appBuilder.Map("/socket", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration 
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
            //appBuilder.MapSignalR("/socket", new HubConfiguration());
            
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
			
			appBuilder.Use(async (context, next) =>
			{
				// Add Header
				var origin = context.Request.Headers.Get("Origin");
				if(String.Equals(origin, "http://samplairre.progx.org:9000")/* || String.Equals(origin, "http://localhost:9000")*/ || String.Equals(origin, "http://localhost:3000")) {
					context.Response.Headers["Access-Control-Allow-Origin"] = origin;
				
					if (context.Request.Method == "OPTIONS"){

						context.Response.StatusCode = 200;

						context.Response.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Methods", "GET", "POST", "PUT", "DELETE", "OPTIONS");

						context.Response.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Headers", "X-Requested-With", "X-HTTP-Method-Override", "Content-Type", "Accept", "ApiToken");
						return;

					}
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
#endif
