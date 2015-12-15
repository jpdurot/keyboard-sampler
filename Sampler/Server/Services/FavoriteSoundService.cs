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
    }
}
