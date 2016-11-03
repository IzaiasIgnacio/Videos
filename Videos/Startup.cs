using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Videos.Startup))]
namespace Videos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
