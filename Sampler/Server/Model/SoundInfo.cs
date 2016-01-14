using System.Runtime.Serialization;
using Newtonsoft.Json;
using SQLite;
using System;

namespace Sampler.Server.Model
{
    [DataContract]
    public class SoundInfo
    {
        private int _id;
        private string _name;

        private bool _isFavorite;
        private int _playedCount;

        private string _uri;
        private string _imageUri;

        public SoundInfo() { }

        public SoundInfo(int id, string name, string uri, string imageUri)
        {
            _id = id;
            _name = name;
            _uri = uri;
            _imageUri = imageUri;
            _playedCount = 0;
            Horodate = new TimeSpan(DateTime.Now.Ticks);
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
        [Ignore]
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { _isFavorite = value; }
        }

        [DataMember]
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        [DataMember]
        public string ImageUri
        {
            get { return _imageUri; }
            set { _imageUri = value; }
        }

        [DataMember]
        [Ignore]
        public int PlayedCount
        {
            get { return _playedCount; }
            set { _playedCount = value; }
        }

        [DataMember]
        public TimeSpan Horodate { get; set; }
    }
}
