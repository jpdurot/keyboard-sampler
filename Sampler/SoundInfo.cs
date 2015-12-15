using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Sampler
{
    [DataContract]
    public class SoundInfo
    {
        private int _id;
        private string _name;
        private bool _isFavorite;

        private string _uri;
        
        public SoundInfo(int id, string name, string uri)
        {
            _id = id;
            _name = name;
            _uri = uri;
        }

        [DataMember]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { _isFavorite = value; }
        }

        [JsonIgnore]
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }
    }
}
