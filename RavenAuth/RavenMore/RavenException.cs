using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Providers.Raven.RavenMore
{
    public class RavenException : Exception
    {
        public RavenException()
        {
        }

        public RavenException(String message)
            : base(message)
        {
        }
    }
}
