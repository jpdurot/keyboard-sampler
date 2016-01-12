using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.SignalR;
using Sampler.Properties;
using Sampler.Server.Model;
using Sampler.Server.Model.Contract;

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

            trophy = new Trophy();
            trophy.Name = "Est-ce que c'est vraiment sérieux ?";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue 2500 sons";
            trophy.IsTrophyUnlocked = g => CheckNumberOfGamePlayed(g, 2500);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "Chuuuuut on t'a dit !!!";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue un son alors que le son est coupé";
            trophy.IsTrophyUnlocked = CheckIfUserHasPlayedWhileMuted;
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "T'es pas couché ??";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue un son tardivement";
            trophy.IsTrophyUnlocked = 
                g => CheckPlayedSoundAfterAndBefore(g, new TimeSpan(1, 0, 0), new TimeSpan(4, 0, 0));
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "T'es déjà levé ??";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Joue un son dès potron-minet (ou presque)";
            trophy.IsTrophyUnlocked = g => CheckPlayedSoundAfterAndBefore(g, new TimeSpan(5, 0, 0), new TimeSpan(7, 0, 0));
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "Tu l'as calmé :D";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Mute un son en cours de lecture";
            trophy.IsTrophyUnlocked = g => CheckMutedSound(g, 1);
            trophy.Id = _trophyCurrentId;
            trophy.Image = new BitmapImage(new Uri("/Images/Trophy/Bronze_32.png", UriKind.RelativeOrAbsolute));
            AddTrophyToDictionary(trophy);

            trophy = new Trophy();
            trophy.Name = "Lucky Luke du mute";
            trophy.Type = Model.Types.TrophyType.Bronze;
            trophy.Description = "Mute 50 sons en cours de lecture";
            trophy.IsTrophyUnlocked = g => CheckMutedSound(g, 50);
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

        public IEnumerable<TrophyInfo> GetAllTrophyInfos()
        {
            var trophyInfos = new List<TrophyInfo>();
            foreach (Trophy trophy in AllTrophies.Values)
            {
                trophyInfos.Add(new TrophyInfo(trophy.Id, trophy.Name, trophy.Description));
            }
            return trophyInfos;
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
            }
        }

        internal void UpdateUserTrophyInfos(int userId, List<TrophyInfo> trophyInfos)
        {
            foreach (var trophyInfo in trophyInfos)
            {
                trophyInfo.IsAchieved = true;
            }

            foreach (Trophy trophy in CurrentUsersTrophiesToAchieve[userId])
            {
                var trophyInfo = trophyInfos.FirstOrDefault(s => s.Id == trophy.Id);
                if (trophyInfo != null)
                {
                    trophyInfo.IsAchieved = false;
                }
            }
        }

        #region IsTrophyUnlocked Methods

        private static bool CheckNumberOfGamePlayed(User user, int step)
        {
            return user.PlaySoundCount >= step;
        }

        private static bool CheckIfUserHasPlayedWhileMuted(User user)
        {
            return
                DataBaseService.Current.Db.Table<Activity>()
                    .Any(a => a.UserId == user.Id && a.Type == ActivityType.PlaySoundWhileMuted);
        }

        private static bool CheckPlayedSoundBefore(User user, TimeSpan timeSpan)
        {
            return
                DataBaseService.Current.Db.Table<Activity>()
                    .Any(
                        a =>
                            a.UserId == user.Id && a.Type == ActivityType.PlaySound && a.Horodate.Hours < timeSpan.Hours);
        }

        private static bool CheckPlayedSoundAfterAndBefore(User user, TimeSpan after, TimeSpan before)
        {
            return
                DataBaseService.Current.Db.Table<Activity>()
                    .Any(
                        a =>
                            a.UserId == user.Id && a.Type == ActivityType.PlaySound &&
                            a.Horodate.Hours < before.Hours && a.Horodate.Hours >= after.Hours);
        }

        private static bool CheckMutedSound(User user, int limit)
        {
            return user.Infos.MutedSounds >= limit;
        }

        #endregion
    }
}
