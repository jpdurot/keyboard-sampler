using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sampler
{
    [DataContract]
    public class SoundInfo
    {
        private int _id;
        private string _name;

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

        [JsonIgnore]
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }
    }
}
