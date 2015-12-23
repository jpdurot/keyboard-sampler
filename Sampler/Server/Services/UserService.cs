using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sampler.Server.Controllers;
using Sampler.Server.Model;
using Sampler.Server.Utils;

namespace Sampler.Server.Services
{
    public class UserService
    {
        #region Singleton
        private static UserService _instance;

        public static UserService Current
        {
            get
            {
                return (_instance ?? (_instance = new UserService()));
            }
        }
        #endregion

        private const int ChatMaxHistory = 10;

        private readonly List<ChatMessage> _chatHistory;
        public List<ChatMessage> ChatHistory
        {
            get { return _chatHistory; }
        }

        public UserService()
        {
            _chatHistory = new List<ChatMessage>();
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
        /// Add a sound to the user list
        /// </summary>
        /// <param name="soundId">the sound id</param>
        /// <param name="user">the user</param>
        /// <returns>true if successfully updated, false instead</returns>
        public void AddPlayedSound(User user, int soundId)
        {
            user.PlaySoundCount++;
            DataBaseService.Current.UpdateData(user);
        }

        /// <summary>
        /// Modify the user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public bool ModifyPassword(User user, string oldPassword, string newPassword)
        {
            if (string.Equals(user.Password, Encryption.HashMd5(oldPassword), StringComparison.OrdinalIgnoreCase))
            {
                user.Password = Encryption.HashMd5(newPassword);
                DataBaseService.Current.UpdateData(user);
                return true;
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

        /// <summary>
        /// Return all users from database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAllUsers()
        {
            return DataBaseService.Current.Db.Table<User>().ToList();
        }

        /// <summary>
        /// Modify user profil
        /// </summary>
        /// <param name="userFromContext">old user</param>
        /// <param name="newUser">new user</param>
        /// <returns></returns>
        public bool ModifyUserProfil(User userFromContext, User newUser)
        {
            userFromContext.PlayingProfil = newUser.PlayingProfil;
            userFromContext.AllowBroadcastSounds = newUser.AllowBroadcastSounds;

            return DataBaseService.Current.Db.Update(userFromContext) > 0;
        }
    }
}
