using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
//using Owin.Security.Providers.Raven.Messages;

namespace Owin.Security.Providers.Raven {
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class RavenAuthenticatedContext : BaseContext {
         /// <summary>
        /// Initializes a <see cref="RavenAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="accessToken">Flick access toke</param>
        //public RavenAuthenticatedContext(IOwinContext context, AccessToken accessToken)
        public RavenAuthenticatedContext(IOwinContext context)
            : base(context)
        {
            //FullName = accessToken.FullName;
            //UserId = accessToken.UserId;
            //UserName = accessToken.UserName;
            //AccessToken = accessToken.Token;
            //AccessTokenSecret = accessToken.TokenSecret;
        }

        /// <summary>
        /// Gets user full name
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the Raven user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the Raven username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the Raven access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Raven access token secret
        /// </summary>
        public string AccessTokenSecret { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}
