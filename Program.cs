#if DOTNETCORE
using Microsoft.AspNetCore;

namespace Sampler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

        host.Run();
        }
    }
}
#endif
