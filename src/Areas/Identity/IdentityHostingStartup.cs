using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(LinkedLanguages.Areas.Identity.IdentityHostingStartup))]
namespace LinkedLanguages.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}