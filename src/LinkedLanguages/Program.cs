using LinkedLanguages.BL;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;


namespace LinkedLanguages
{
    public class Program
    {
        public static void Main(string[] args)
        {

            WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
            });

            builder.Services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme, options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuers = new string[] { "https://linkedlanguages.azurewebsites.net/" }
                };
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAppUserProvider, AppUserProvider>();
            builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<UnusedUserWordPairsQuery>();
            builder.Services.AddTransient<LanguageFacade>();
            builder.Services.AddTransient<WordPairFacade>();
            builder.Services.AddTransient<TestWordPairFacade>();
            builder.Services.AddTransient<WordPairPump>();
            builder.Services.AddTransient<WordPairsSparqlQuery>();
            builder.Services.AddTransient<PairsStatisticsSparqlQuery>();
            builder.Services.AddTransient<WordDefinitionSparqlQuery>();
            builder.Services.AddTransient<WordPairsUserQuery>();
            builder.Services.AddTransient<ApprovedWordPairsQuery>();
            builder.Services.AddTransient<RejectedWordPairsQuery>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
            builder.Services.Configure<SparqlEndpointOptions>(builder.Configuration.GetSection("SparqlEndpointOptions"));
            builder.Services.AddApplicationInsightsTelemetry(options: new ApplicationInsightsServiceOptions { ConnectionString = builder.Configuration.GetValue<string>("APPINSIGHTS_CONNECTIONSTRING") });

            WebApplication? app = builder.Build();

            using (IServiceScope? scope = app.Services.CreateScope())
            {
                ApplicationDbContext? dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}