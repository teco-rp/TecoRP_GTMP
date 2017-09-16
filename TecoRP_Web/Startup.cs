using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TecoRP_Web.Startup))]
namespace TecoRP_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
