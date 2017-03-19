using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InfoFinder.Startup))]
namespace InfoFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
