using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if DOTNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Sampler.Server.Model;
using Sampler.Server.Services;
using Sampler.Server.Utils;
using Sampler.Server.Model.Response;
using Sampler.Server.Model.Contract;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/Sounds")]
    #if DOTNETCORE
    public class SoundController : Controller
    #else
    public class SoundController : ApiController
    #endif
    {
        private static readonly Sampler1 _sampler = Sampler1.Current;
        private static readonly IHubContext _soundsHubContext = GlobalHost.ConnectionManager.GetHubContext<SoundsHub>();

        // GET
        [HttpGet]
        [Route("play/{id}")]
        [CustomAuthorization]
        [Quota]
        [Trophy]
        public void Get(int id)
        {
            PlaySoundInternal(id);
        }

        // POST
        [HttpPost]
        [Route("play/{id}")]
        [CustomAuthorization]
        [Quota]
        public void Post(int id)
        {
            PlaySoundInternal(id);
        }

        private void PlaySoundInternal(int id)
        {
            SoundInfo soundInfo = _sampler.GetSoundInfo(id);
            User user = Request.GetUserContext();
            ActivityType activityType;
            if (!_sampler.IsMuted)
            {
                // We add sound only if it's not muted
                activityType = ActivityType.PlaySound;
                UserService.Current.AddPlayedSound(user, id);
                SoundService.Current.AddPlayedSound(soundInfo);
            }
            else
            {
                activityType = ActivityType.PlaySoundWhileMuted;
            }

            Activity a = new Activity()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                Type = activityType,
                UserId = user.Id,
                Information = id.ToString(CultureInfo.InvariantCulture)
            };
            HistoryService.Current.AddActivity(a);
            _sampler.PlaySound(id, false);
            _soundsHubContext.Clients.All.notifyNewSound(soundInfo, user.Name, _sampler.IsMuted);
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
        [Trophy]
        public MuteResponse Mute()
        {
            // Sound is up and will be cut
            if (!_sampler.IsMuted && _sampler.Config.GetPlayers().Any(p => p.IsPlaying()))
            {
                Request.GetUserContext().Infos.MutedSounds++;
            }
            _sampler.IsMuted = !_sampler.IsMuted;

            _soundsHubContext.Clients.All.syncIsMuted(_sampler.IsMuted, Request.GetUserContext().Name);

            return new MuteResponse() { IsMuted = _sampler.IsMuted };
        }

        [HttpPost]
        [Route("add")]
        [CustomAuthorization]
        [Trophy]
        public AddSoundResponse AddSound([FromBody] AddSoundBody body)
        {
            string errorResponse = SoundService.Current.AddSound(body);
            return new AddSoundResponse() { IsAdded = string.IsNullOrEmpty(errorResponse), Error = errorResponse };
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
        public IEnumerable<PlayedSound> GetLastPlayedSounds(int count)
        {
            var latestSounds = new List<PlayedSound>();
            var latestActivities = HistoryService.Current.GetLatestPlayedSounds(count);
            var allUsers = UserService.Current.GetAllUsers();

            foreach (var latestActivity in latestActivities)
            {
                int soundId;
                if (int.TryParse(latestActivity.Information, out soundId))
                {
                    var playedSound = new PlayedSound()
                    {
                        SoundName = _sampler.GetSoundInfo(soundId).Name,
                        UserName = allUsers.First(u => u.Id == latestActivity.UserId).Name,
                        Time = new DateTime(latestActivity.Horodate.Ticks).ToString("dd/MM/yyyy HH:mm:ss")
                    };
                    latestSounds.Add(playedSound);
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

    #region Latest played sound

    public class PlayedSound
    {
        [JsonProperty("soundname")]
        public string SoundName { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("playtime")]
        public string Time { get; set; }
    }

    #endregion
}
