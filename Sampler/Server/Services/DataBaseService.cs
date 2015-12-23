using System;
using System.Collections.Generic;
using System.Threading;
using Sampler.Server.Model;
using Sampler.Server.Model.Types;
using SQLite;

namespace Sampler.Server.Services
{
    public class DataBaseService : IDisposable
    {
        private const string DbName = "samplere.sqlite";
        private SQLiteConnection _dataBase;

        private readonly object _lockQueue = new object();
        private readonly Queue<DatabaseOperation> _objectToOperate = new Queue<DatabaseOperation>();
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
                _objectToOperate.Enqueue(new DatabaseOperation(obj, DatabaseOperationType.Insert));
                _resetEvent.Set();
            }
        }

        public void UpdateData(object obj)
        {
            lock (_lockQueue)
            {
                _objectToOperate.Enqueue(new DatabaseOperation(obj, DatabaseOperationType.Update));
                _resetEvent.Set();
            }
        }

        public void DeleteData(object obj)
        {
            lock (_lockQueue)
            {
                _objectToOperate.Enqueue(new DatabaseOperation(obj, DatabaseOperationType.Delete));
                _resetEvent.Set();
            }
        }
        
        private void Loop()
        {
            while (!_stopRequested)
            {
                _resetEvent.WaitOne();
                DatabaseOperation obj = null;
                lock (_lockQueue)
                {
                    if (_objectToOperate.Count > 0)
                        obj = _objectToOperate.Dequeue();
                }
                if (obj != null)
                {
                    switch (obj.Type)
                    {
                        case DatabaseOperationType.Insert :
                            _dataBase.Insert(obj.Object);
                            break;
                        case DatabaseOperationType.Update :
                            _dataBase.Update(obj.Object);
                            break;
                        case DatabaseOperationType.Delete :
                            _dataBase.Delete(obj.Object);
                            break;
                    }
                }
                lock (_lockQueue)
                {
                    if (_objectToOperate.Count == 0)
                        _resetEvent.Reset();
                }
            }
        }
    }
}
