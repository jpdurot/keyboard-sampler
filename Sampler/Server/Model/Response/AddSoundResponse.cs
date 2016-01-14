using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sampler.Server.Model.Response
{
    public class AddSoundResponse
    {
        [JsonProperty("isadded")]
        public bool IsAdded { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
