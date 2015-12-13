using System;
using System.Collections.Generic;
using Sampler.Server.Model;

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
        private readonly Dictionary<int, string> _tokensByUser;

        public AuthenticationService()
        {
            _tokens = new Dictionary<string, int>();
            _tokensByUser = new Dictionary<int, string>();
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

        public User Authenticate(string userName, string password)
        {
            return DbConnectionService.Current.Authenticate(userName, password);
        }

        public string GetAuthenticationToken(int userId)
        {
            string oldToken;
            if (_tokensByUser.TryGetValue(userId, out oldToken))
            {
                _tokens.Remove(oldToken);
                _tokensByUser.Remove(userId);
            }
            string token = Guid.NewGuid().ToString("N");
            _tokens[token] = userId;
            _tokensByUser[userId] = token;

            return token;

        }
    }
}
