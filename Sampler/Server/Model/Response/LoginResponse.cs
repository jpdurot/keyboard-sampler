using Newtonsoft.Json;

namespace Sampler.Server.Model
{
    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("playingProfil")]
        public PlayingProfilType PlayingProfil { get; set; }

        [JsonProperty("allowBroadcastSounds")]
        public bool AllowBroadcastSounds { get; set; }

        [JsonProperty("playSoundCount")]
        public int PlaySoundCount { get; set; }
    }
}
