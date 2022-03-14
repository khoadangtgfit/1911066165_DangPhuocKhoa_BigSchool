using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_1911066165_DangPhuocKhoa_BigSchool.Startup))]
namespace _1911066165_DangPhuocKhoa_BigSchool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
