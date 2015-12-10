using System;
using Sampler.Server.Model;
using SQLite;

namespace Sampler.Server.Services
{
    public class DbConnectionService : IDisposable
    {
        private const string DbName = "samplere.sqlite";
        private SQLiteConnection _dataBase;

        #region Singleton
        private static DbConnectionService _instance;

        public static DbConnectionService Current
        {
            get
            {
                return (_instance ?? (_instance = new DbConnectionService()));
            }
        }
        #endregion

        private DbConnectionService()
        {
            
        }

        public void Connect()
        {
            _dataBase = new SQLiteConnection(DbName);

            _dataBase.CreateTable<User>();
            _dataBase.DeleteAll<User>();
            _dataBase.Insert(new User() {Name = "JP"});
        }

        public void Dispose()
        {
            _dataBase.Close();
        }
    }
}
