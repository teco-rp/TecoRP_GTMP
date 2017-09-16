using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TecoRP_Website.Startup))]
namespace TecoRP_Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
