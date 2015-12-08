using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        public void Test()
        {

        }
    }
}
