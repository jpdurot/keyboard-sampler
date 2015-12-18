using System.Collections.Generic;
using System.Linq;
using Sampler.Server.Model;

namespace Sampler.Server.Services
{
    public class HistoryService
    {

        #region Singleton
        private static HistoryService _instance;

        public static HistoryService Current
        {
            get
            {
                return (_instance ?? (_instance = new HistoryService()));
            }
        }
        #endregion

        private HistoryService()
        {
        }

        public void AddActivity(Activity a)
        {
            DataBaseService.Current.AddData(a);
        }

        /// <summary>
        /// Retrieve the latest played sounds
        /// </summary>
        /// <param name="count">number of sounds to retrieve</param>
        /// <returns></returns>
        public IEnumerable<Activity> GetLatestPlayedSounds(int count)
        {
            var query = DataBaseService.Current.Db.Table<Activity>()
                .Where(act => act.Type == ActivityType.PlaySound)
                .OrderByDescending(act => act.Horodate)
                .Take(count);

            return query.ToList();
        }
    }
}
