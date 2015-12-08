using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Sampler
{
    internal class Sampler1
    {

        private static Sampler1 _instance;
        private Configuration _config;
        private bool _isMuted;
        private Player _currentSound;

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
            _isMuted = false;
        }


        private void LoadConfiguration()
        {
            _config = Configuration.Parse(XDocument.Load("Configuration.xml").Root.Elements().First());
        }

        
        public void PlaySound(int soundId, bool repeat)
        {
            if (_isMuted)
                return;

            _currentSound = _config.GetSound(soundId);
            if (_currentSound != null)
            {
                if (!_currentSound.IsLooping)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => _currentSound.Play(repeat));
                }
                else
                {
                    Dispatcher.CurrentDispatcher.Invoke(_currentSound.Stop);
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

        public void MuteOrUnmute()
        {
            _isMuted = !_isMuted;
            _currentSound.Stop();
        }
    }
}
