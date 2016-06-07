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
    
    #if DOTNETCORE
        [Route("api/user")]
        public class UserController : Controller
    #else
        [RoutePrefix("api/user")]
        public class UserController : ApiController
    #endif
    {
        #if DOTNETCORE
            [HttpGet]
        #else
            [HttpGet]
            [Route("")]
            [CustomAuthorization]  
        #endif
        public IEnumerable<User> GetUserList()
        {
            var users = AuthenticationService.Current.GetAuthenticatedtUsersList();

            return users;
        }

        #if DOTNETCORE
            [HttpGet("messages")]
        #else
            [HttpGet]
            [Route("")]
            [CustomAuthorization]  
        #endif
        public IEnumerable<ChatMessage> GetChatHistory()
        {
            return UserService.Current.ChatHistory;
        }

        #if DOTNETCORE
            [HttpPost("modifyProfil")]
        #else
            [HttpPost]
            [Route("modifyprofil")]
            [CustomAuthorization] 
        #endif
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
