using System;
using SQLite;

namespace Sampler.Server.Model
{
    public class Activity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public TimeSpan Horodate { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public ActivityType Type { get; set; }

        public string Information { get; set; }

    }
}
