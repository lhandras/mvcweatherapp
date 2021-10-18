using Microsoft.Owin;
using Owin;
using WeatherApplication;

[assembly: OwinStartup(typeof(Startup))]
namespace WeatherApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
