using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DOTNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif
using Sampler.Server.Model;
using Sampler.Server.Model.Contract;
using Sampler.Server.Services;
using Sampler.Server.Utils;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/Trophies")]
    #if DOTNETCORE
    public class TrophyController : Controller
    #else
    public class TrophyController : ApiController
    #endif
    {
        // GET
        [HttpGet]
        [Route("info")]
        [CustomAuthorization]
        public IEnumerable<TrophyInfo> GetTrophyInfo()
        {
            IEnumerable<TrophyInfo> trophyInfos = TrophyService.Current.GetAllTrophyInfos();

            TrophyService.Current.UpdateUserTrophyInfos(Request.GetUserContext().Id, trophyInfos.ToList());

            return trophyInfos;
        }
    }
}
