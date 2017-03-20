using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BrainySearch.Startup))]
namespace BrainySearch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
