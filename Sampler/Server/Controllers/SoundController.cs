using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private static readonly Sampler1 _sampler = Sampler1.Current;
        private static readonly IHubContext _soundsHubContext = GlobalHost.ConnectionManager.GetHubContext<SoundsHub>();

        // GET
        [HttpGet]
        [Route("play/{id}")]
        [CustomAuthorization]
        [Quota]
        public void Get(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            _soundsHubContext.Clients.All.notifyNewSound(soundInfo, Request.GetUserContext().Name, _sampler.IsMuted);
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
            _soundsHubContext.Clients.All.notifyNewSound(soundInfo, Request.GetUserContext().Name, _sampler.IsMuted);
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
            IEnumerable<SoundInfo> soundsInfo = _sampler.GetSoundsInfo();

            SoundService.Current.UpdateSoundsInfo(Request.GetUserContext().Id, soundsInfo.ToList());

            return soundsInfo;
        }

        // POST
        [HttpPost]
        [Route("mute")]
        [CustomAuthorization]
        [Quota]
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
        public FavoriteResponse MarkSoundFavorite([FromBody] FavoriteBody favoriteBody)
        {
            if (SoundService.Current.IsFavorite(Request.GetUserContext().Id, favoriteBody.SoundId))
            {
                SoundService.Current.RemoveFromFavorite(Request.GetUserContext().Id, favoriteBody.SoundId);
                return new FavoriteResponse() {IsFavorite = false};
            }
            
            SoundService.Current.AddToFavorite(Request.GetUserContext().Id, favoriteBody.SoundId);
            return new FavoriteResponse() { IsFavorite = true };
        }

        // GET
        [HttpGet]
        [Route("latest/{count}")]
        [CustomAuthorization]
        public IEnumerable<SoundInfo> GetLastPlayedSounds(int count)
        {
            var latestSounds = new List<SoundInfo>();
            var latestActivities = HistoryService.Current.GetLatestPlayedSounds(count);

            foreach (var latestActivity in latestActivities)
            {
                int soundId;
                if (int.TryParse(latestActivity.Information, out soundId))
                {
                    latestSounds.Add(_sampler.GetSoundInfo(soundId));
                }
            }

            return latestSounds;
        }
    }

    #region Favorite classes
    public class FavoriteBody
    {
        [JsonProperty("soundId")]
        public int SoundId { get; set; }
    }

    public class FavoriteResponse
    {
        [JsonProperty("isFavorite")]
        public bool IsFavorite { get; set; }
    }
    #endregion
}
