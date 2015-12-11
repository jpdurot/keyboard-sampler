using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sampler.Server.Model;
using Sampler.Server.Services;

namespace Sampler.Server.Utils
{
    class CustomAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private const string AuthorizationHeaderName = "ApiToken";
        public bool AllowMultiple { get; private set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //string authorizationHeaderValue = 
            if (actionContext.Request.Headers.Contains(AuthorizationHeaderName))
            {
                var authValue = actionContext.Request.Headers.GetValues(AuthorizationHeaderName).FirstOrDefault();

                if (authValue != null)
                {
                    var connectedUser = AuthenticationService.Current.GetUser(authValue);
                    if (connectedUser == null)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        // User is authenticated
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, new LoginResponse() { UserName = connectedUser.Name });
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                
            }
        }
    }
}
