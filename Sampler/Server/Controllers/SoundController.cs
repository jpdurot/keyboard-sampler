using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Sampler.Server.Model;
using Sampler.Server.Services;
using Sampler.Server.Utils;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/Sounds")]
    public class SoundController : ApiController
    {

        private static Sampler1 _sampler = Sampler1.Current;
        private static IHubContext _soundsHubContext = GlobalHost.ConnectionManager.GetHubContext<SoundsHub>();

        // GET
        [HttpGet]
        [Route("play/{id}")]
        [CustomAuthorization]
        public void Get(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            _soundsHubContext.Clients.All.addNewMessageToPage((soundInfo!=null ? soundInfo.Name : "??"), Request.GetUserContext().Name);
            Activity a = new Activity()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                Type = ActivityType.PlaySound,
                UserId = Request.GetUserContext().Id,
                Information = id.ToString(CultureInfo.InvariantCulture)
            };
            HistoryManager.Current.AddActivity(a);
            _sampler.PlaySound(id, false);
        }

        // POST
        [HttpPost]
        [Route("play/{id}")]
        [CustomAuthorization]
        public void Post(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            _soundsHubContext.Clients.All.addNewMessageToPage((soundInfo!=null ? soundInfo.Name : "??"), Request.GetUserContext().Name);
            Activity a = new Activity()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                Type = ActivityType.PlaySound,
                UserId = Request.GetUserContext().Id
            };
            HistoryManager.Current.AddActivity(a);
            _sampler.PlaySound(id, false);
        }

        // GET
        [HttpGet]
        [Route("info")]
        [CustomAuthorization]
        public IEnumerable<SoundInfo> GetSoundsInfo()
        {
            return _sampler.GetSoundsInfo();
        }

        // POST
        [HttpPost]
        [Route("mute")]
        [CustomAuthorization]
        public MuteResponse Mute()
        {
            _sampler.IsMuted = !_sampler.IsMuted;

            _soundsHubContext.Clients.All.syncIsMuted(_sampler.IsMuted);

            return new MuteResponse() { IsMuted = _sampler.IsMuted };
        }

        // GET
        [HttpGet]
        [Route("ismuted")]
        [CustomAuthorization]
        public MuteResponse IsMuted()
        {
            return new MuteResponse() { IsMuted = _sampler.IsMuted };
        }
    }
}
