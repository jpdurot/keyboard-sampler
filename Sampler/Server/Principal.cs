using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Sampler.Server
{
    class Principal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; private set; }

        public Principal(IIdentity identity)
        {
            Identity = identity;
        }
    }

    class Identity : IIdentity
    {
        public string Name { get; private set; }
        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public Identity(string name)
        {
            Name = name;
            IsAuthenticated = true;
        }
    }
}
