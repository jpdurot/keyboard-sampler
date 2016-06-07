using System.Linq;
using System.Net;
using System.Net.Http;
#if DOTNETCORE
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Filters;
#else
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
#endif

using Sampler.Server.Model;
using Sampler.Server.Services;

namespace Sampler.Server.Utils
{
    class CustomAuthorizationAttribute : AuthorizationFilterAttribute
    {
        public const string AuthorizationHeaderName = "ApiToken";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //string authorizationHeaderValue = 
            if (actionContext.Request.Headers.Contains(AuthorizationHeaderName))
            {
                var authValue = actionContext.Request.Headers.GetValues(AuthorizationHeaderName).FirstOrDefault();

                if (authValue != null)
                {
                    var connectedUser = AuthenticationService.Current.GetUserId(authValue);
                    if (connectedUser == -1)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        // User is authenticated
                        User user = UserService.Current.GetUser(connectedUser);
                        actionContext.Request.SetUserContext(user);
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
