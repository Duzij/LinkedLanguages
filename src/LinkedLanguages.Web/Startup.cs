
using LinkedLanguages.BL;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using System.Security.Claims;

namespace LinkedLanguages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            _ = services.AddDatabaseDeveloperPageExceptionFilter();
            _ = services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            _ = services.Configure<IdentityOptions>(options =>
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

            _ = services.AddTransient<IAppUserProvider, AppUserProvider>();
            _ = services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            _ = services.AddAuthentication()
                .AddIdentityServerJwt();

            _ = services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme, options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuers = new string[] { "https://linkedlanguages.azurewebsites.net/" }
                };
            });

            _ = services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            _ = services.AddControllersWithViews();
            _ = services.AddRazorPages();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            _ = services.AddMemoryCache();

            _ = services.AddTransient<UnusedUserWordPairsQuery>();

            _ = services.AddTransient<LanguageFacade>();
            _ = services.AddTransient<WordPairFacade>();
            _ = services.AddTransient<WordPairPump>();
            _ = services.AddTransient<SparqlPairsQuery>();
            _ = services.AddTransient<SparqlPairsStatisticsQuery>();

            _ = services.AddTransient<IEmailSender, EmailSender>();
            _ = services.Configure<AuthMessageSenderOptions>(Configuration);
            _ = services.Configure<SparqlEndpointOptions>(Configuration.GetSection("SparqlEndpointOptions"));

            _ = services.AddApplicationInsightsTelemetry(options: new ApplicationInsightsServiceOptions { ConnectionString = Configuration.GetValue<string>("APPINSIGHTS_CONNECTIONSTRING") });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
                _ = app.UseMigrationsEndPoint();
            }
            else
            {
                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();

            }

            dbContext.Database.Migrate();

            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();
            app.UseSpaStaticFiles();

            _ = app.UseRouting();

            _ = app.UseAuthentication();
            _ = app.UseIdentityServer();
            _ = app.UseAuthorization();
            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                _ = endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }
}
