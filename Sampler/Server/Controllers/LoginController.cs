using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Sampler.Server.Model;
using Sampler.Server.Services;
using Sampler.Server.Utils;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Login([FromBody] UserLogin userInfo)
        {
            var user = AuthenticationService.Current.Authenticate(userInfo.UserName, userInfo.Password);
            if (user != null)
            {
                var authenticationToken = AuthenticationService.Current.GetAuthenticationToken(user.Id);
                return Request.CreateResponse(HttpStatusCode.OK, new LoginResponse { Token = authenticationToken, UserName = userInfo.UserName});

            }

            return Request.CreateResponse(HttpStatusCode.NotFound);

        }

        [HttpGet]
        [Route("test")]
        [CustomAuthorization]
        public LoginResponse Test()
        {
            return new LoginResponse() {UserName = Request.GetUserContext().Name};
        }

        [HttpGet]
        [Route("logout")]
        [CustomAuthorization]
        public HttpResponseMessage Logout()
        {
            if (AuthenticationService.Current.Disconnect(Request.GetUserContext()))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("password")]
        [CustomAuthorization]
        public HttpResponseMessage ModifyPassword([FromBody] ModifyPasswordBody passwordBody)
        {
            if (AuthenticationService.Current.ModifyPassword(Request.GetUserContext(), passwordBody.OldPassword,
                passwordBody.NewPassword))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }

    #region Modify password

    public class ModifyPasswordBody
    {
        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }

        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }

    #endregion
}
