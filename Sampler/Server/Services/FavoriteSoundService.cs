using System.Collections.Generic;
using System.Linq;
using Sampler.Server.Model;

namespace Sampler.Server.Services
{
    public class FavoriteSoundService
    {
        #region Singleton

        private static FavoriteSoundService _instance;

        public static FavoriteSoundService Current
        {
            get
            {
                return (_instance ?? (_instance = new FavoriteSoundService()));
            }
        }

        #endregion

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
                DataBaseService.Current.Db.Delete<FavoriteSound>(favoriteToRemove.Id);
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
    }
}
