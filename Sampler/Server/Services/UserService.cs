﻿using System;
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

        #region SoundsHubUser property and methods
        private int _nbSoundsHubUsers;

        /// <summary>
        /// Add a new connected User
        /// </summary>
        internal void AddConnectedSoundsHubUser()
        {
            _nbSoundsHubUsers++;
        }

        /// <summary>
        /// Remove a connected User
        /// </summary>
        internal void RemoveConnectedSoundsHubUser()
        {
            _nbSoundsHubUsers--;
        }

        /// <summary>
        /// Get the connected User count
        /// </summary>
        /// <returns>user count</returns>
        internal int GetConnectedSoundsHubUserCount()
        {
            return _nbSoundsHubUsers;
        }

        #endregion

        private readonly List<ChatMessage> _chatHistory;
        private Dictionary<int, UserInfos> _userInfos; 
        public List<ChatMessage> ChatHistory
        {
            get { return _chatHistory; }
        }

        public UserService()
        {
            _chatHistory = new List<ChatMessage>();
            _userInfos = new Dictionary<int, UserInfos>();
            foreach (User user in GetAllUsers())
            {
                _userInfos.Add(user.Id, new UserInfos());
            }
        }

        /// <summary>
        /// Gets the user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            User user = DataBaseService.Current.Db.Table<User>()
                .FirstOrDefault(
                    u =>
                        u.Id == userId);
            user.Infos = GetUserInfos(user.Id);
            return user;
        }

        private UserInfos GetUserInfos(int id)
        {
            return _userInfos[id];
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
