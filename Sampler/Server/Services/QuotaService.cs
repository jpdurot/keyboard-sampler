using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampler.Server.Services
{
    public class QuotaService
    {
        private const double Interval = 10.0;
        private const int AllowedNumberOfCalls = 2;

        private object _lockUsages = new object();

        #region Singleton
        private static QuotaService _instance;

        public static QuotaService Current
        {
            get
            {
                return (_instance ?? (_instance = new QuotaService()));
            }
        }
        #endregion

        // Dictionary of TimeSpan (represented in seconds) list, sorted by time, and grouped by user id and function names
        private readonly Dictionary<string, Dictionary<int, SortedList<double, double>>> _usages;


        public QuotaService()
        {
            _usages = new Dictionary<string, Dictionary<int, SortedList<double, double>>>();
        }
        
        public bool AddUse(string functionName, int userId)
        {
            lock (_lockUsages)
            {
                Dictionary<int, SortedList<double, double>> _usagesByFunction;
                if (!_usages.TryGetValue(functionName, out _usagesByFunction))
                {
                    _usagesByFunction = new Dictionary<int, SortedList<double, double>>();
                    _usages[functionName] = _usagesByFunction;
                }
                SortedList<double, double> _usagesByUser;
                if (!_usagesByFunction.TryGetValue(userId, out _usagesByUser))
                {
                    _usagesByUser = new SortedList<double, double>();
                    _usagesByFunction[userId] = _usagesByUser;
                }


                // Check number of times this function was called
                double currentTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                double minTime = currentTime - Interval;

                // Remove elements older than Interval
                while (_usagesByUser.Any() && _usagesByUser.First().Value < minTime)
                {
                    _usagesByUser.RemoveAt(0);
                }

                if (_usagesByUser.Count >= AllowedNumberOfCalls)
                {
                    return false;
                }
                else
                {
                    _usagesByUser.Add(currentTime, currentTime);
                    return true;
                }
            }





        }
    }
}
