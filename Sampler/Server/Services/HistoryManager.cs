using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sampler.Server.Model;

namespace Sampler.Server.Services
{
    public class HistoryManager
    {

        #region Singleton
        private static HistoryManager _instance;

        public static HistoryManager Current
        {
            get
            {
                return (_instance ?? (_instance = new HistoryManager()));
            }
        }
        #endregion

        private readonly object _lockQueue = new object();

        private readonly Queue<Activity> _activities = new Queue<Activity>();

        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        private Thread _worker;
        private volatile bool _stopRequested;

        private HistoryManager()
        {
            _worker = new Thread(Loop);
            _worker.Start();
        }

        public void AddActivity(Activity a)
        {
            lock (_lockQueue)
            {
                _activities.Enqueue(a);
                _resetEvent.Set();
            }
        }

        public void Stop()
        {
            _stopRequested = true;
            _resetEvent.Set();
        }


        private void Loop()
        {
            while (!_stopRequested)
            {
                _resetEvent.WaitOne();
                Activity activity = null;
                lock (_lockQueue)
                {
                    if (_activities.Count > 0)
                        activity = _activities.Dequeue();
                }
                if (activity != null)
                {
                    DbConnectionService.Current.AddActivity(activity);
                }
                lock (_lockQueue)
                {
                    if (_activities.Count == 0)
                        _resetEvent.Reset();
                }
            }
        }
    }
}
