using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Sampler.Server.Model.Types;

namespace Sampler.Server.Model
{
    public class Trophy
    {
        public string Name { get; set; }
        public TrophyType Type { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public Func<User, bool> IsTrophyUnlocked { get; set; }
        public BitmapImage Image { get; set; }
    }
}
