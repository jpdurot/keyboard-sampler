using System;
using System.Collections.Generic;
using System.Linq;
using Sampler.Server.Controllers;
using Sampler.Server.Model;
using Sampler.Server.Utils;
using SQLite;

namespace Sampler.Server.Services
{
    public class AuthenticationService
    {
        #region Singleton
        private static AuthenticationService _instance;

        public static AuthenticationService Current
        {
            get
            {
                return (_instance ?? (_instance = new AuthenticationService()));
            }
        }
        #endregion

        private readonly Dictionary<string, int> _tokens;
        private readonly Dictionary<int, List<string>> _tokensByUser;
        private readonly List<User> _authenticatedUsers;

        public AuthenticationService()
        {
            _tokens = new Dictionary<string, int>();
            _tokensByUser = new Dictionary<int, List<string>>();
            _authenticatedUsers = new List<User>();
        }

        /// <summary>
        /// Returns user id corresponding to the token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>User identifier if it exists, otherwise -1</returns>
        public int GetUserId(string token)
        {
            int userId;
            if (_tokens.TryGetValue(token, out userId))
            {
                return userId;
            }
            return -1;
        }

        /// <summary>
        /// Gets the authenticated users list
        /// </summary>
        /// <returns>List of User</returns>
        public IList<User> GetAuthenticatedtUsersList()
        {
            return _authenticatedUsers;
        }

        /// <summary>
        /// Gets the authentication token associated to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetAuthenticationToken(int userId)
        {
            List<string> oldTokens;
            if (!_tokensByUser.TryGetValue(userId, out oldTokens))
            {
                _tokensByUser[userId] = new List<string>();
            }
            string token = Guid.NewGuid().ToString("N");
            _tokens[token] = userId;
            _tokensByUser[userId].Add(token);

            return token;

        }

        /// <summary>
        /// Authenticate a user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        public User Authenticate(string userName, string password)
        {
            string cryptedPassword = Encryption.HashMd5(password);
            User currentUser = DataBaseService.Current.Db.Table<User>().FirstOrDefault(u => u.Name == userName && u.Password == cryptedPassword);
            if (currentUser != null)
                _authenticatedUsers.Add(currentUser);
            return currentUser;
        }
        
        /// <summary>
        /// Disconnect an user from the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Disconnect(User user)
        {
            foreach (var token in _tokensByUser[user.Id])
            {
                _tokens.Remove(token);
            }
            _authenticatedUsers.Remove(user);

            return _tokensByUser.Remove(user.Id);
        }
    }
}
