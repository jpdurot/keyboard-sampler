using System;
using SQLite;

namespace Sampler.Server.Model
{
    public class UserTrophy
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public int TrophyId { get; set; }

        public TimeSpan Horodate { get; set; }
    }
}
