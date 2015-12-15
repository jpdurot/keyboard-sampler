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
    }
}
