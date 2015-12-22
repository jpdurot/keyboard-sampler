using System;
using System.IO;
using NAudio.Wave;

namespace Sampler
{
    public class Player
    {
        private IWavePlayer _mediaPlayer;
        private WaveStream _soundReader;
        private WaveChannel32 _soundChannel;

        private readonly string _soundFile;

        public bool IsLooping { get; private set; }

        public Player(Uri sound)
        {
            _soundFile = sound.OriginalString;
        }

        public void SetDevice(Guid deviceId)
        {
            Reset();

            if (_soundFile.EndsWith(".wav"))
            {
                _soundReader = new WaveFileReader(_soundFile);
                
            }
            else if (_soundFile.EndsWith(".mp3"))
            {
                _soundReader = new Mp3FileReader(_soundFile);
            }
            else
            {
                throw new NotSupportedException("Invalid format " + _soundFile);
            }

            _soundChannel = new WaveChannel32(_soundReader) { PadWithZeroes = false };
            _soundReader.Seek(0, SeekOrigin.Begin);
            _mediaPlayer = new DirectSoundOut(deviceId);
            _mediaPlayer.Init(_soundChannel);
            _mediaPlayer.PlaybackStopped += OnPlaybackStopped;
        }

        public void Reset()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.PlaybackStopped -= OnPlaybackStopped;
                _mediaPlayer.Stop();
                _mediaPlayer = null;
            }
            if (_soundReader != null)
            {
                _soundReader.Close();
                _soundReader = null;
            }
        }
        
        public void Play(bool loop)
        {
            if (_mediaPlayer != null)
            {
                IsLooping = loop;
                _soundReader.Seek(0, SeekOrigin.Begin);
                _mediaPlayer.Play();
            }
        }

        public void Stop()
        {
            if (_mediaPlayer != null)
            {
                IsLooping = false;
                _mediaPlayer.Stop(); 
            }
        }

        public void SetVolume(float scale)
        {
            _soundChannel.Volume = scale;
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (IsLooping)
            {
                Play(true);
            }
        }
    }
}
