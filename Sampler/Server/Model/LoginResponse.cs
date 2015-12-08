using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sampler.Server.Model
{
    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
