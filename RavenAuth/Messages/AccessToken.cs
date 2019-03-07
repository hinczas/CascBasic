// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace Owin.Security.Providers.Raven.Messages
{
    /// <summary>
    /// Raven access token
    /// </summary>
    public class AccessToken : RequestToken
    {
        /// <summary>
        /// Gets or sets the Raven User ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the Raven User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Raven User Full Name
        /// </summary>
        public string FullName { get; set; }
    }
}
