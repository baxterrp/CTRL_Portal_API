using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Configuration;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Middleware;
using CTRL.Portal.API.Services;
using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Repositories;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CTRL.Portal.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentMigrator(this IServiceCollection services, string connectionString)
        {
            var migrationAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsClass && t.Namespace == "CTRL.Portal.API.CustomMigrations")
                .Select(x => x.Assembly).ToArray();

            services.AddFluentMigratorCore()
                .ConfigureRunner(runner => runner.AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(migrationAssemblies).For.Migrations());

            return services;
        }
    }

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
            services.AddControllers();

            var emailConfig = new EmailConfiguration();
            var appSettingsEmailConfig = Configuration.GetSection("EmailConfiguration");
            appSettingsEmailConfig.Bind(emailConfig);
            services.AddSingleton(emailConfig);

            var authConfig = new AuthenticationConfiguration();
            var appSettingsAuthConfig = Configuration.GetSection("JWT");
            appSettingsAuthConfig.Bind(authConfig);
            services.AddSingleton(authConfig);

            services.AddTransient(sp => new AuthenticationParameters
            {
                Issuer = authConfig.ValidIssuer,
                Audience = authConfig.ValidAudience,
                Key = authConfig.Secret
            });

            var connectionString = Configuration.GetConnectionString("CTRL_Connection");
            var dbConfig = new DatabaseConfiguration
            {
                ConnectionString = connectionString
            };
            services.AddSingleton(dbConfig);

            services.AddFluentMigrator(connectionString);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiNames.ApiAuthenticationScheme;
                    options.DefaultChallengeScheme = ApiNames.ApiAuthenticationScheme;
                    options.DefaultSignInScheme = ApiNames.ApiAuthenticationScheme;
                })
                .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(ApiNames.ApiAuthenticationScheme, "Api Authenticaiton", null);

            services.AddTransient<IAuthenticationTokenManager, AuthenticationTokenManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IUserSettingsService, UserSettingsService>();
            services.AddSingleton<IUserSettingsRepository, UserSettingsRepository>();
            services.AddSingleton<IEmailProvider, EmailProvider>();
            services.AddSingleton<IUtilityManager, UtilityManager>();

            services.AddCors(sp => sp.AddPolicy("StandardPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner runner)
        {
            app.UseRouting();
            app.UseCors("StandardPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseMiddleware(typeof(ApiPortalMiddleware));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            runner.MigrateUp();
        }
    }
}
