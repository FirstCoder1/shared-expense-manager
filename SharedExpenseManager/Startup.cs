using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SharedExpenseManager.Startup))]
namespace SharedExpenseManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
