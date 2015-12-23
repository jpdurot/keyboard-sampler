using Microsoft.Owin.StaticFiles.ContentTypes;

namespace Sampler.Server.Utils
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add("samplere.appcache", "text/cache-manifest");
        }
    }
}
