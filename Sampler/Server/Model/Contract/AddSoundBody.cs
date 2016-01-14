using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sampler.Server.Model.Contract
{
    public class AddSoundBody
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageuri")]
        public string ImageUri { get; set; }
    }
}
