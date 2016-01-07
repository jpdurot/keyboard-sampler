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
         
        public static Configuration Parse(XElement element)
        {
            Configuration config  = new Configuration(element.Attribute("name").Value);

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
                Player p = new Player(soundUri);
                config._mappings.Add(keyCode, p);
                config._soundsInfo.Add(new SoundInfo(keyCode, name, soundUri.OriginalString, imageUri));
            }

            return config;
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
