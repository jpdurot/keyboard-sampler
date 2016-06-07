#if DOTNETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Sampler.Server.Model;
using Sampler.Server.Services;

namespace Sampler.Server.Utils
{
    public class TrophyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext httpActionContext)
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
#endif
