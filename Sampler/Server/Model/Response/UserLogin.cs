using Newtonsoft.Json;

namespace Sampler.Server.Model
{
    public class UserLogin
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
