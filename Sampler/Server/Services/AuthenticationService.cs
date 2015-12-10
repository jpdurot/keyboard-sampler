using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private readonly Dictionary<string, User> _tokens;
        private readonly Dictionary<User, string> _tokensByUser;

        public AuthenticationService()
        {
            _tokens = new Dictionary<string, User>();
            _tokensByUser = new Dictionary<User, string>();
        }
        public User GetUser(string token)
        {
            User user;
            _tokens.TryGetValue(token, out user);
            return user;
        }

        public User Authenticate(string userName, string password)
        {
            if (userName == "JP" && password == "JP")
            {
                return new User() { Name = "JP" };
            }
            return null;
        }

        public string GetAuthenticationToken(User user)
        {
            string oldToken = string.Empty;
            if (_tokensByUser.TryGetValue(user, out oldToken))
            {
                _tokens.Remove(oldToken);
                _tokensByUser.Remove(user);
            }
            string token = Guid.NewGuid().ToString("N");
            _tokens[token] = user;
            _tokensByUser[user] = token;

            return token;

        }
        



    }
}
