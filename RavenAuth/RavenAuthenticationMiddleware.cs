using System;
using System.Globalization;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin.Security.Providers.Raven.Messages;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace Owin.Security.Providers.Raven
{
    public class RavenAuthenticationMiddleware : AuthenticationMiddleware<RavenAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public RavenAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app,
            RavenAuthenticationOptions options)
            : base(next, options)
        {
            _logger = app.CreateLogger<RavenAuthenticationMiddleware>();

            if (Options.Provider == null)
                Options.Provider = new RavenAuthenticationProvider();

            if (Options.StateDataFormat == null)
            {
                var dataProtector = app.CreateDataProtector(
                    typeof(RavenAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new SecureDataFormat<RequestToken>(
                    Serializers.RequestToken,
                    dataProtector,
                    TextEncodings.Base64Url);
            }

            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();

            _httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024*1024*10
            };
        }

        /// <summary>
        ///     Provides the <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> object for processing
        ///     authentication-related requests.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> configured with the
        ///     <see cref="T:Owin.Security.Providers.Raven.RavenAuthenticationOptions" /> supplied to the constructor.
        /// </returns>
        protected override AuthenticationHandler<RavenAuthenticationOptions> CreateHandler()
        {
            return new RavenAuthenticationHandler(_httpClient, _logger);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(RavenAuthenticationOptions options)
        {
            var handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator == null) return handler;
            // Set the cert validate callback
            var webRequestHandler = handler as WebRequestHandler;
            if (webRequestHandler == null)
            {
                throw new InvalidOperationException(Resources.Exception_ValidatorHandlerMismatch);
            }
            webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;

            return handler;
        }
    }
}