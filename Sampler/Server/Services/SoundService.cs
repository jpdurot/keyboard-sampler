using System.Collections.Generic;
using System.Linq;
using Sampler.Server.Model;
using System;

namespace Sampler.Server.Services
{
    public class SoundService
    {
        #region Singleton

        private static SoundService _instance;

        public static SoundService Current
        {
            get
            {
                return (_instance ?? (_instance = new SoundService()));
            }
        }

        #endregion

        private Dictionary<int, int> soundDictionary;
        private int _lastSoundId;

        public void SetLastSoundId(int id)
        {
            _lastSoundId = id;
        }

        internal SoundService()
        {
            soundDictionary = new Dictionary<int, int>();

            // Get all Play Sound activity (only ActivityType.PlaySound)
            var activityList = DataBaseService.Current.Db.Table<Activity>().Where(t => t.Type == ActivityType.PlaySound);
            foreach (Activity activity in activityList)
            {
                int soundId = int.Parse(activity.Information);
                AddPlayedSound(soundId);
            }
        }

        private void AddPlayedSound(int soundId)
        {
            if (!soundDictionary.ContainsKey(soundId))
                soundDictionary.Add(soundId, 1);
            else
                soundDictionary[soundId]++;
        }

        public void AddToFavorite(int userId, int soundId)
        {
            var favoriteSound = new FavoriteSound() {UserId = userId, SoundId = soundId};
            DataBaseService.Current.AddData(favoriteSound);
        }

        public void RemoveFromFavorite(int userId, int soundId)
        {
            var favoriteToRemove = DataBaseService.Current.Db.Table<FavoriteSound>()
                .FirstOrDefault(f => f.UserId == userId && f.SoundId == soundId);

            if (favoriteToRemove != null)
            {
                DataBaseService.Current.DeleteData(favoriteToRemove);
            }
        }

        public bool IsFavorite(int userId, int soundId)
        {
            var favoriteToRemove = DataBaseService.Current.Db.Table<FavoriteSound>()
                .FirstOrDefault(f => f.UserId == userId && f.SoundId == soundId);

            return favoriteToRemove != null;
        }

        public void UpdateSoundsInfo(int userId, List<SoundInfo> soundsInfo)
        {
            foreach (var soundInfo in soundsInfo)
            {
                soundInfo.IsFavorite = false;
                if (soundDictionary.ContainsKey(soundInfo.Id))
                {
                    // Prevent replacing by 0 when playing a sound while calculating
                    if (soundInfo.PlayedCount < soundDictionary[soundInfo.Id])
                        soundInfo.PlayedCount = soundDictionary[soundInfo.Id];
                }
            }

            var userFavorites = DataBaseService.Current.Db.Table<FavoriteSound>().Where(f => f.UserId == userId);
            foreach (var favoriteSound in userFavorites)
            {
                var soundInfo = soundsInfo.FirstOrDefault(s => s.Id == favoriteSound.SoundId);
                if (soundInfo != null)
                {
                    soundInfo.IsFavorite = true;
                }
            }
        }

        internal void AddPlayedSound(SoundInfo sound)
        {
            sound.PlayedCount++;
            if (!soundDictionary.ContainsKey(sound.Id))
                soundDictionary.Add(sound.Id, sound.PlayedCount);
            else
                soundDictionary[sound.Id] = sound.PlayedCount;
        }

        internal string AddSound(Model.Contract.AddSoundBody body)
        {
            var sound = new SoundInfo(_lastSoundId, body.Name, body.Uri, body.ImageUri);
            string retour = String.Empty;
            try
            {
                DataBaseService.Current.AddData(sound);
            }
            catch (Exception ex)
            {
                retour = ex.Message;
            }
            return retour;
        }
    }
}
