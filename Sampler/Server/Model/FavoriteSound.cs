using SQLite;

namespace Sampler.Server.Model
{
    public class FavoriteSound
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public int SoundId { get; set; }
    }
}
