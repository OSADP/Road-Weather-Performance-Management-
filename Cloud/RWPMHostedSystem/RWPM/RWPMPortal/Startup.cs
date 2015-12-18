using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RWPMPortal.Startup))]
namespace RWPMPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
