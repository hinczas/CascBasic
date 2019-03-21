using AutoMapper;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
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

            Mapper.Initialize(cfg => {
                cfg.CreateMap<Institution, InstManageViewModel>();
            });
        }
    }
}
