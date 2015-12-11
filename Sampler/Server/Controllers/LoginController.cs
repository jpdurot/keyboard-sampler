using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
                var authenticationToken = AuthenticationService.Current.GetAuthenticationToken(user);
                return Request.CreateResponse(HttpStatusCode.OK, new LoginResponse { Token = authenticationToken });

            }

            return Request.CreateResponse(HttpStatusCode.NotFound);

        }

        [HttpGet]
        [Route("test")]
        [CustomAuthorization]
        public LoginResponse Test()
        {
            if (Request.Headers.Contains(CustomAuthorizationAttribute.AuthorizationHeaderName))
            {
                var authValue =
                    Request.Headers.GetValues(CustomAuthorizationAttribute.AuthorizationHeaderName).FirstOrDefault();

                if (authValue != null)
                {
                    var connectedUser = AuthenticationService.Current.GetUser(authValue);
                    if (connectedUser != null)
                    {
                        return new LoginResponse() {UserName = connectedUser.Name};
                    }
                }
            }
            return new LoginResponse();
        }
    }
}
