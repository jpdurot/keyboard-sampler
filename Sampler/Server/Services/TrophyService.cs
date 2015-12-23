using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.SignalR;
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
        private static readonly IHubContext _soundsHubContext = GlobalHost.ConnectionManager.GetHubContext<SoundsHub>();
        internal Dictionary<int, Trophy> AllTrophies;
        internal Dictionary<int, List<Trophy>> CurrentUsersTrophiesToAchieve;
        private int _trophyCurrentId;

        public TrophyService()
        {
            // TODO : Add the trophy list loading
            _trophyCurrentId = 0;
            AllTrophies = new Dictionary<int, Trophy>();
            CurrentUsersTrophiesToAchieve = new Dictionary<int, List<Trophy>>();
            PrepareTrophies();
            ComputeUserTrophyDictionary();
        }

        private void ComputeUserTrophyDictionary()
        {
            //Prepare full dictionary
            List<Trophy> allTrophyListId = AllTrophies.Values.ToList();

            foreach (User user in UserService.Current.GetAllUsers())
            {
                var listCopy = new List<Trophy>();
                foreach (Trophy trophy in allTrophyListId)
                {
                    listCopy.Add(trophy);
                }
                CurrentUsersTrophiesToAchieve.Add(user.Id, listCopy);
            }

            // Remove already checked trophies
            foreach (UserTrophy ut in DataBaseService.Current.Db.Table<UserTrophy>().ToList())
            {
                CurrentUsersTrophiesToAchieve[ut.UserId].Remove(AllTrophies[ut.TrophyId]);
            }
        }

        private static bool CheckNumberOfGamePlayed(User user, int step)
        {
            return user.PlaySoundCount >= step;
        }

        private void PrepareTrophies()
        {
            var trophy = new Trophy();
            trophy.Name = "Balance ton premier son";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue un son";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 1);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "Tu commences à aimer ?";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue 10 sons";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 10);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "Tu kiff !";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue 100 sons";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 100);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "On ne t'arrête plus";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue 1000 sons";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 1000);
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
            _soundsHubContext.Clients.All.broadcastTrophy(userName, trophy.Name);
        }

        public void AddTrophyToUser(User user, int id)
        {
            //TrophyService.AddTrophyToUser(user.Id, id);
            UserTrophy userTrophy = new UserTrophy()
            {
                Horodate = new TimeSpan(DateTime.Now.Ticks),
                TrophyId = id,
                UserId = user.Id
            };
            DataBaseService.Current.AddData(userTrophy);
            Trophy trophy = AllTrophies[id];
            ToastNewTrophy(user.Name, trophy);
        }

        internal void UpdateTrophy(string actionId, User user)
        {
            lock (_lockUsages)
            {
                List<Trophy> trophyAchievedList = new List<Trophy>();
                foreach (Trophy trophy in CurrentUsersTrophiesToAchieve[user.Id])
                {
                    //Trophy trophy = AllTrophies[trophyId];
                    if (trophy.IsTrophyUnlocked(user))
                    {
                        AddTrophyToUser(user, trophy.Id);
                        trophyAchievedList.Add(trophy);
                    }
                }

                //Remove Trophies from list
                foreach (Trophy trophy in trophyAchievedList)
                {
                    CurrentUsersTrophiesToAchieve[user.Id].Remove(trophy);
                }
                //TODO : Improve message use
                //List<Trophy> trophyAchievedList = new List<Trophy>();
                //foreach (Trophy trophy in )
                //{
                //    if (trophy.IsTrophyUnlocked(ServiceGamer.Gamer))
                //    {
                //        AddTrophyToGamer(trophy.Id);
                //        trophyAchievedList.Add(trophy);
                //    }
                //}
            }
        }
    }
}
