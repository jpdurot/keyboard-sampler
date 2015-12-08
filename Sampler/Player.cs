using System;
using System.IO;
using NAudio.Wave;

namespace Sampler
{
    public class Player
    {
        private IWavePlayer _mediaPlayer;
        private WaveFileReader _soundReader;
        private WaveChannel32 _soundChannel;

        private string _soundFile;
        //private MediaPlayer _mediaPlayer;

        public bool IsLooping { get; private set; }

        public Player(Uri sound)
        {
            /*using (var sr = new StreamReader(File.OpenRead(sound.AbsolutePath)))
            {
                StreamWriter sw = new StreamWriter(_soundStream);
                sw.Write(sr.ReadToEnd());
            }*/
            _soundFile = sound.OriginalString;
           
        }

        public void SetDevice(Guid deviceId)
        {
            Reset();
            _soundReader = new WaveFileReader(_soundFile);
            _soundChannel = new WaveChannel32(_soundReader) { PadWithZeroes = false };
            _soundReader.Seek(0, SeekOrigin.Begin);
            _mediaPlayer = new DirectSoundOut(deviceId);
            _mediaPlayer.Init(_soundChannel);

            _mediaPlayer.PlaybackStopped += _mediaPlayer_PlaybackStopped;
        }

        public void Reset()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.PlaybackStopped -= _mediaPlayer_PlaybackStopped;
                _mediaPlayer.Stop();
                _mediaPlayer = null;
            }
            if (_soundReader != null)
            {
                _soundReader.Close();
                _soundReader = null;
            }
        }

        void _mediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (IsLooping)
            {
                Play(true);
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
    }
}
