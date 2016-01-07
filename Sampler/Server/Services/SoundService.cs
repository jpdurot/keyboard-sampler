using System.Collections.Generic;
using System.Linq;
using Sampler.Server.Model;

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

        internal SoundService()
        {
            soundDictionary = new Dictionary<int, int>();
            var activityList = DataBaseService.Current.Db.Table<Activity>();
            foreach (Activity activity in activityList)
            {
                int soundId = int.Parse(activity.Information);
                AddPlayedSound(soundId);
            }
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
                    soundInfo.PlayedCount = soundDictionary[soundInfo.Id];
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

        internal void AddPlayedSound(int soundId)
        {
            if (!soundDictionary.ContainsKey(soundId))
                soundDictionary.Add(soundId, 1);
            else
                soundDictionary[soundId]++;
        }
    }
}
