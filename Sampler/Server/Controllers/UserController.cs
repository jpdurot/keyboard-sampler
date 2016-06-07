using System.Collections.Generic;
using System.Net;
using System.Net.Http;
#if DOTNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif
using Newtonsoft.Json;
using Sampler.Server.Model;
using Sampler.Server.Services;
using Sampler.Server.Utils;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/user")]
    #if DOTNETCORE
    public class UserController : Controller
    #else
    public class UserController : ApiController
    #endif
    {
        [HttpGet]
        [Route("")]
        [CustomAuthorization]
        public IEnumerable<User> GetUserList()
        {
            var users = AuthenticationService.Current.GetAuthenticatedtUsersList();

            return users;
        }

        [HttpGet]
        [Route("messages")]
        [CustomAuthorization]
        public IEnumerable<ChatMessage> GetChatHistory()
        {
            return UserService.Current.ChatHistory;
        }

        [HttpPost]
        [Route("modifyprofil")]
        [CustomAuthorization]
        public HttpResponseMessage ModifyUserProfil([FromBody] User user)
        {
            if (UserService.Current.ModifyUserProfil(Request.GetUserContext(), user))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }

    public class ChatMessage
    {
        [JsonProperty("username")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Message { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }
    }

}
