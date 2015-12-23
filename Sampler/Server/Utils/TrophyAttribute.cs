using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sampler.Server.Model;
using Sampler.Server.Services;

namespace Sampler.Server.Utils
{
    public class TrophyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext httpActionContext)
        {
            string actionId = string.Concat(httpActionContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, "/",
                httpActionContext.ActionContext.ActionDescriptor.ActionName);

            // Check current user
            User currentUser = httpActionContext.Request.GetUserContext();

            if (currentUser != null)
            {
                TrophyService.Current.UpdateTrophy(actionId, currentUser);
            }
        }
    }
}
