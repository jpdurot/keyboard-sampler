using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Sampler
{
    internal class Sampler1
    {

        private static Sampler1 _instance;
        private Configuration _config;


        public static Sampler1 Current
        {
            get
            {
                return (_instance?? (_instance = new Sampler1()));
            }
        }

        public Configuration Config
        {
            get { return _config; }
        }

        public Sampler1()
        {
            LoadConfiguration();
        }


        private void LoadConfiguration()
        {
            _config = Configuration.Parse(XDocument.Load("Configuration.xml").Root.Elements().First());
        }

        
        public void PlaySound(int soundId, bool repeat)
        {
            Player sound = _config.GetSound(soundId);
            if (sound != null)
            {
                if (!sound.IsLooping)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => sound.Play(repeat));
                }
                else
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => sound.Stop());
                }
            }
        }

        public void ChangeDevice(Guid deviceId)
        {
            foreach (var player in _config.GetPlayers())
            {
                player.SetDevice(deviceId);
            }
        }

        public IList<SoundInfo> GetSoundsInfo()
        {
            return _config.SoundsInfo;
        }
    }
}
