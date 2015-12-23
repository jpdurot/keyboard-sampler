using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sampler.Server.Model.Contract
{
    [DataContract]
    public class TrophyInfo
    {
        private int _id;
        private string _name;
        private string _description;
        private bool _isAchieved;

        private string _uri;

        public TrophyInfo(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
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
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [DataMember]
        public bool IsAchieved
        {
            get { return _isAchieved; }
            set { _isAchieved = value; }
        }
    }
}
