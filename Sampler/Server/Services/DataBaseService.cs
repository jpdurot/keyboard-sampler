using System;
using System.Collections.Generic;
using System.Threading;
using Sampler.Server.Model;
using SQLite;

namespace Sampler.Server.Services
{
    public class DataBaseService : IDisposable
    {
        private const string DbName = "samplere.sqlite";
        private SQLiteConnection _dataBase;

        private readonly object _lockQueue = new object();
        private readonly Queue<object> _objectToInsert = new Queue<object>();
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private volatile bool _stopRequested;

        #region Singleton
        private static DataBaseService _instance;

        public static DataBaseService Current
        {
            get
            {
                return (_instance ?? (_instance = new DataBaseService()));
            }
        }
        #endregion

        public SQLiteConnection Db
        {
            get { return _dataBase; }
        }

        private DataBaseService()
        {
            Thread worker = new Thread(Loop);
            worker.Start();
        }

        public void Connect()
        {
            _dataBase = new SQLiteConnection(DbName);
        }
        
        public void Dispose()
        {
            _stopRequested = true;
            _resetEvent.Set();
            
            _dataBase.Close();
        }

        public void AddData(object obj)
        {
            lock (_lockQueue)
            {
                _objectToInsert.Enqueue(obj);
                _resetEvent.Set();
            }
        }
        
        private void Loop()
        {
            while (!_stopRequested)
            {
                _resetEvent.WaitOne();
                object obj = null;
                lock (_lockQueue)
                {
                    if (_objectToInsert.Count > 0)
                        obj = _objectToInsert.Dequeue();
                }
                if (obj != null)
                {
                    _dataBase.Insert(obj);
                }
                lock (_lockQueue)
                {
                    if (_objectToInsert.Count == 0)
                        _resetEvent.Reset();
                }
            }
        }
    }
}
