using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
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
        [Quota]
        public void Get(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            _soundsHubContext.Clients.All.addNewSoundMessageToPage(soundInfo, Request.GetUserContext().Name, _sampler.IsMuted);
            Activity a = new Activity()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                Type = ActivityType.PlaySound,
                UserId = Request.GetUserContext().Id,
                Information = id.ToString(CultureInfo.InvariantCulture)
            };
            HistoryService.Current.AddActivity(a);
            _sampler.PlaySound(id, false);
        }

        // POST
        [HttpPost]
        [Route("play/{id}")]
        [CustomAuthorization]
        [Quota]
        public void Post(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            _soundsHubContext.Clients.All.addNewSoundMessageToPage(soundInfo, Request.GetUserContext().Name, _sampler.IsMuted);
            Activity a = new Activity()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                Type = ActivityType.PlaySound,
                UserId = Request.GetUserContext().Id
            };
            HistoryService.Current.AddActivity(a);
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

            _soundsHubContext.Clients.All.syncIsMuted(_sampler.IsMuted, Request.GetUserContext().Name);

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

        // POST
        [HttpPost]
        [Route("favorite")]
        [CustomAuthorization]
        public void MarkSoundFavorite([FromBody] FavoriteBody favoriteBody)
        {
            if (FavoriteSoundService.Current.IsFavorite(Request.GetUserContext().Id, favoriteBody.SoundId))
            {
                FavoriteSoundService.Current.RemoveFromFavorite(Request.GetUserContext().Id, favoriteBody.SoundId);
            }
            else
            {
                FavoriteSoundService.Current.AddToFavorite(Request.GetUserContext().Id, favoriteBody.SoundId);
            }
        }
    }

    public class FavoriteBody
    {
        [JsonProperty("soundId")]
        public int SoundId { get; set; }
    }
}
