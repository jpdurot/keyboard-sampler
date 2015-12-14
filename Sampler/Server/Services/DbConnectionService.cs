using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        }

        public User Authenticate(string userName, string password)
        {
            string cryptedPassword = CreateMd5(password);

            return _dataBase.Table<User>()
                .FirstOrDefault(
                    u =>
                        u.Name == userName &&
                        u.Password == cryptedPassword);
        }

        public void Dispose()
        {
            _dataBase.Close();
        }

        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public User GetUser(int connectedUser)
        {
            return _dataBase.Table<User>()
                .FirstOrDefault(
                    u =>
                        u.Id == connectedUser);
        }
    }
}
