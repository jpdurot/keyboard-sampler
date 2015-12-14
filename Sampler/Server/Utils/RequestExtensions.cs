using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sampler.Server.Model;

namespace Sampler.Server.Utils
{
    public static class RequestExtensions
    {

        public static void SetUserContext(this HttpRequestMessage request, User user)
        {
            request.Properties["User"] = user;
        }

        public static User GetUserContext(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("User"))
            {
                return (User) request.Properties["User"];
            }
            return null;
        }
    }
}
