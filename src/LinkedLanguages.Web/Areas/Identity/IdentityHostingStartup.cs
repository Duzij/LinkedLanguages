[assembly: HostingStartup(typeof(LinkedLanguages.Web.Areas.Identity.IdentityHostingStartup))]
namespace LinkedLanguages.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}