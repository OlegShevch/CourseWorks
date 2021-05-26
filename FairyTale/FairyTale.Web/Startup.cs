using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FairyTale.Web.Startup))]
namespace FairyTale.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
