using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BigSchoolLancuoi.Startup))]
namespace BigSchoolLancuoi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
