using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

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

            appBuilder.UseWebApi(config);

            var fileSystem = new PhysicalFileSystem("www");

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem
            };

            appBuilder.UseFileServer(options);

            config.EnsureInitialized();
        }
    }
}
