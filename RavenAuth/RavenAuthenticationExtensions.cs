using System;

namespace Owin.Security.Providers.Raven
{
    public static class RavenAuthenticationExtensions
    {
        public static IAppBuilder UseRavenAuthentication(this IAppBuilder app,
            RavenAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            app.Use(typeof(RavenAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseRavenAuthentication(this IAppBuilder app)
        {
            return app.UseRavenAuthentication(new RavenAuthenticationOptions());
        }
    }
}