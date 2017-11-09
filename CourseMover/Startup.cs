using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseMover.Startup))]
namespace CourseMover
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}