using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sampler
{
    [RoutePrefix("api/Sounds")]
    public class SoundController : ApiController
    {

        private static Sampler1 _sampler = Sampler1.Current;

        // GET
        [HttpGet]
        [Route("play/{id}")]
        public void Get(int id)
        {
            _sampler.PlaySound(id, false);
        }

        // POST
        [HttpPost]
        [Route("play/{id}")]
        public void Post(int id)
        {
            _sampler.PlaySound(id, false);
        }

        // GET
        [HttpGet]
        [Route("info")]
        public IEnumerable<SoundInfo> GetSoundsInfo()
        {
            return _sampler.GetSoundsInfo();
        }
    }

}
