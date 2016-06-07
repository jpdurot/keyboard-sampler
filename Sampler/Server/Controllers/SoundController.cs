using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if DOTNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using Microsoft.AspNet.SignalR;

#endif
using Newtonsoft.Json;
using Sampler.Server.Model;
using Sampler.Server.Services;
using Sampler.Server.Utils;
using Sampler.Server.Model.Response;
using Sampler.Server.Model.Contract;

namespace Sampler.Server.Controllers
{
    
    #if DOTNETCORE
    [Route("api/Sounds")]
    public class SoundController : Controller
    #else
    [RoutePrefix("api/Sounds")]
    public class SoundController : ApiController
    #endif
    {
        private static readonly Sampler1 _sampler = Sampler1.Current;
        private static readonly IHubContext _soundsHubContext = GlobalHost.ConnectionManager.GetHubContext<SoundsHub>();

        // GET
        #if DOTNETCORE
            [HttpGet("play/{id]")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #else
            [HttpGet]
            [Route("play/{id}")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #endif
        public void Get(int id)
        {
            PlaySoundInternal(id);
        }

        // POST
        #if DOTNETCORE
            [HttpPost("play/{id]")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #else
            [HttpPost]
            [Route("play/{id}")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #endif
        
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
        #if DOTNETCORE
            [HttpGet("info")]
            [CustomAuthorization]
        #else
            [HttpGet]
            [Route("info")]
            [CustomAuthorization]
        #endif
        public IEnumerable<SoundInfo> GetSoundsInfo()
        {
            IEnumerable<SoundInfo> soundsInfo = _sampler.GetSoundsInfo();

            SoundService.Current.UpdateSoundsInfo(Request.GetUserContext().Id, soundsInfo.ToList());

            return soundsInfo;
        }

        // POST
        #if DOTNETCORE
            [HttpPost("mute")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #else
            [HttpPost]
            [Route("mute")]
            [CustomAuthorization]
            [Quota]
            [Trophy]
        #endif
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

        #if DOTNETCORE
            [HttpPost("add")]
            [CustomAuthorization]
            [Trophy]
        #else
            [HttpPost]
            [Route("add")]
            [CustomAuthorization]
            [Trophy]
        #endif
        public AddSoundResponse AddSound([FromBody] AddSoundBody body)
        {
            string errorResponse = SoundService.Current.AddSound(body);
            return new AddSoundResponse() { IsAdded = string.IsNullOrEmpty(errorResponse), Error = errorResponse };
        }

        // GET
        #if DOTNETCORE
            [HttpGet("ismuted")]
            [CustomAuthorization]
        #else
            [HttpGet]
            [Route("ismuted")]
            [CustomAuthorization]
        #endif
        public MuteResponse IsMuted()
        {
            return new MuteResponse() { IsMuted = _sampler.IsMuted };
        }

        // POST
        #if DOTNETCORE
            [HttpPost("favorite")]
            [CustomAuthorization]
        #else
            [HttpPost]
            [Route("favorite")]
            [CustomAuthorization]
        #endif
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
        #if DOTNETCORE
            [HttpGet("latest/{count}")]
            [CustomAuthorization]
        #else
            [HttpGet]
            [Route("latest/{count}")]
            [CustomAuthorization]
        #endif
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
