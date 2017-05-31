using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StoreCatalogue.Startup))]
namespace StoreCatalogue
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
