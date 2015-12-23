using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampler.Server.Model.Types;

namespace Sampler.Server.Model
{
    public class DatabaseOperation
    {
        public object Object { get; set; }
        public DatabaseOperationType Type { get; set; }

        public DatabaseOperation(object o, DatabaseOperationType type)
        {
            Object = o;
            Type = type;
        }
    }
}
