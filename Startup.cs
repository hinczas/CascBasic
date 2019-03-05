using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CascBasic.Startup))]
namespace CascBasic
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
