using Sampler.Server.Model;
using Sampler.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace Sampler
{
    internal class Configuration
    {
        private readonly Dictionary<int, Player> _mappings;

        private List<SoundInfo> _soundsInfo; 

        public string Name
        {
            get; private set; 
        }

        public List<SoundInfo> SoundsInfo
        {
            get { return _soundsInfo; }
        }

        public SoundInfo GetSoundInfo(int id)
        {
            // TODO Perf 
            foreach (SoundInfo soundInfo in _soundsInfo)
            {
                if (soundInfo.Id == id)
                    return soundInfo;
            }
            return null;
        }

        private Configuration(string name)
        {
            _mappings = new Dictionary<int, Player>();
            Name = name;
            _soundsInfo = new List<SoundInfo>();
        }

        public List<Player> GetPlayers()
        {
            return _mappings.Values.ToList();
        }

        public static Configuration GetConfiguration()
        {
            Configuration config = new Configuration("Configuration");
            var soundList = DataBaseService.Current.Db.Table<SoundInfo>().ToList();
            int maxId = -1;
            foreach (SoundInfo sound in soundList)
            {
                Player p = new Player(new Uri(sound.Uri, UriKind.Relative));
                config._mappings.Add(sound.Id, p);
                config._soundsInfo.Add(sound);
                if (sound.Id > maxId)
                {
                    maxId = sound.Id;
                }
            }
            SoundService.Current.SetLastSoundId(maxId + 1);
            return config;
        }
         
        public static void CheckNewSoundInConfig(XElement element)
        {
            foreach (var child in element.Descendants("Sound"))
            {
                var keyCode = int.Parse(child.Attribute("keyCode").Value);
                var soundUri = new Uri(child.Attribute("path").Value, UriKind.Relative);

                string imageUri = string.Empty;
                if (child.Attribute("img") != null)
                {
                    imageUri = new Uri(child.Attribute("img").Value, UriKind.Relative).OriginalString;
                }
                var name = string.Empty;
                if (child.Attribute("name") != null)
                {
                    name = child.Attribute("name").Value;
                }

                if (!DataBaseService.Current.Db.Table<SoundInfo>().Any(s => s.Id == keyCode))
                {
                    var sound = new SoundInfo(keyCode, name, soundUri.OriginalString, imageUri);
                    DataBaseService.Current.Db.Insert(sound);
                }
            }
        }

        public Player GetSound(int keyCode)
        {
            if (_mappings.ContainsKey(keyCode))
            {
                return _mappings[keyCode];
            }
            else
            {
                return null;
            }
        }

    }
}
