using SQLite;

namespace Sampler.Server.Model
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }
}
