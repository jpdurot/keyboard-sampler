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
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string actionId = string.Concat(actionContext.ControllerContext.ControllerDescriptor.ControllerName, "/",
                actionContext.ActionDescriptor.ActionName);

            // Check current user
            User currentUser = actionContext.Request.GetUserContext();

            if (currentUser != null)
            {
                TrophyService.Current.UpdateTrophy(actionId, currentUser);
            }
        }
    }
}
