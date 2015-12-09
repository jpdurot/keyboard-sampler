using Newtonsoft.Json;

namespace Sampler.Server.Model
{
    public class MuteResponse
    {
        [JsonProperty("ismuted")]
        public bool IsMuted { get; set; }
    }
}
