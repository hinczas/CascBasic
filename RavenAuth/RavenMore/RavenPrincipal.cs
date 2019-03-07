using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Providers.Raven.RavenMore
{
    public class RavenPrincipal : IPrincipal
    {
        #region Instance members
        /// <summary>
        /// 
        /// </summary>
        private RavenIdentity identity;
        private ClaimsPrincipal claims;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public IIdentity Identity
        {
            get { return this.identity; }
        }
        #endregion

        public ClaimsPrincipal Claims
        {
            get
            {
                return this.claims;
            }
        }


        #region IsInRole
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public Boolean IsInRole(string role)
        {
            return false;
        }
        #endregion

        public RavenPrincipal(RavenIdentity identity)
        {
            this.identity = identity;
            //this.claims = claims;
        }
    }
}
