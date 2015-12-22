using System;
using SQLite;

namespace Sampler.Server.Model
{
    public class User : IEquatable<User>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Equals(User otherUser)
        {
            if (otherUser == null) 
                return false;
            return (this.Id.Equals(otherUser.Id));
        } 

        public int PlaySoundCount { get; set; }
    }
}
