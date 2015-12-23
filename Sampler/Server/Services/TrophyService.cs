using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Sampler.Properties;
using Sampler.Server.Model;

namespace Sampler.Server.Services
{
    public class TrophyService
    {
        #region Singleton

        private static TrophyService _instance;

        public static TrophyService Current
        {
            get
            {
                return (_instance ?? (_instance = new TrophyService()));
            }
        }

        #endregion

        private readonly object _lockUsages = new object();
        internal Dictionary<int, Trophy> AllTrophies;
        private int _trophyCurrentId;

        public TrophyService()
        {
            // TODO : Add the trophy list loading
            _trophyCurrentId = 0;
            AllTrophies = new Dictionary<int, Trophy>();
            PrepareTrophies();
        }

        private static bool CheckNumberOfGamePlayed(User user, int step)
        {
            return user.PlaySoundCount >= step;
        }

        private void PrepareTrophies()
        {


            var trophy = new Trophy();
            trophy.Name = "Joue un son";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Balance ton premier son";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 1);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);
        }

        private void AddTrophyToDictionary(Trophy trophy)
        {
            _trophyCurrentId++;
            AllTrophies.Add(trophy.Id, trophy);
        }

        public static void ToastNewTrophy(string userName, Trophy trophy)
        {
            //Try to Toast
        }

        public void AddTrophyToUser(User user, int id)
        {
            //TrophyService.AddTrophyToUser(user.Id, id);
            Trophy trophy = AllTrophies[id];
            ToastNewTrophy(user.Name, trophy);
        }

        internal void UpdateTrophy(string actionId, User user)
        {
            lock (_lockUsages)
            {

            }
        }
    }
}
