using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sampler.Server.Controllers;
using Sampler.Server.Model;
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

        private const int ChatMaxHistory = 10;

        private readonly List<ChatMessage> _chatHistory;
        private readonly Dictionary<string, int> _tokens;
        private readonly Dictionary<int, List<string>> _tokensByUser;
        private readonly List<User> _authenticatedUsers;

        public List<ChatMessage> ChatHistory
        {
            get { return _chatHistory; }
        }

        public AuthenticationService()
        {
            _tokens = new Dictionary<string, int>();
            _tokensByUser = new Dictionary<int, List<string>>();
            _authenticatedUsers = new List<User>();
            _chatHistory = new List<ChatMessage>();
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
        /// Gets the user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            return DataBaseService.Current.Db.Table<User>()
                .FirstOrDefault(
                    u =>
                        u.Id == userId);
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
            string cryptedPassword = HashMd5(password);
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

        /// <summary>
        /// Modify the user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public bool ModifyPassword(User user, string oldPassword, string newPassword)
        {
            if (string.Equals(user.Password, HashMd5(oldPassword), StringComparison.OrdinalIgnoreCase))
            {
                user.Password = HashMd5(newPassword);
                return DataBaseService.Current.Db.Update(user) > 0;
            }
            return false;
        }

        /// <summary>
        ///  Store previous chat in memory
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="message">chat message</param>
        public ChatMessage AddChatMessage(string name, string message)
        {
            if (_chatHistory.Count >= ChatMaxHistory)
            {
                _chatHistory.RemoveAt(0);
            }

            var chatMessage = new ChatMessage()
            {
                Name = name,
                Message = message,
                Time = DateTime.Now.ToString("dd/MM à HH:mm")
            };
            _chatHistory.Add(chatMessage);

            return chatMessage;
        }

        private static string HashMd5(string input)
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
    }
}
