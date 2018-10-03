using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AiCard.Startup))]
namespace AiCard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
