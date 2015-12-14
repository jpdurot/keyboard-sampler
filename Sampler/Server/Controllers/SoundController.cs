using System.Collections.Generic;
using System.Web.Http;
using Sampler.Server.Model;
using Sampler.Server.Utils;

namespace Sampler.Server.Controllers
{
    [RoutePrefix("api/Sounds")]
    public class SoundController : ApiController
    {

        private static Sampler1 _sampler = Sampler1.Current;

        // GET
        [HttpGet]
        [Route("play/{id}")]
        [CustomAuthorization]
        public void Get(int id)
        {
            _sampler.PlaySound(id, false);
        }

        // POST
        [HttpPost]
        [Route("play/{id}")]
        [CustomAuthorization]
        public void Post(int id)
        {
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
