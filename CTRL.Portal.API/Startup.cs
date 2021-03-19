using CTRL.Authentication.Configuration;
using CTRL.Authentication.Contracts;
using CTRL.Authentication.Middleware;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Extensions;
using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Services.EntityContexts;
using CTRL.Portal.Services.Implementation;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTRL.Portal.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddConfigurations(Configuration);

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

            services.AddCustomAuthentication();
            services.AddCustomServices(Configuration);

            services.AddTransient(sp =>
            {
                var authConfig = sp.GetRequiredService<AuthenticationConfiguration>();

                return new AuthenticationParameters
                {
                    Issuer = authConfig.ValidIssuer,
                    Audience = authConfig.ValidAudience,
                    Key = authConfig.Secret
                };
            });

            services.AddCors(sp => sp.AddPolicy("StandardPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
            }));

        }

        public void Configure(
            IApplicationBuilder app, 
            IMigrationRunner runner,
            RoleManager<IdentityRole> roleManager)
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

            IdentityUtilityManager.SeedRoles(roleManager);
        }
    }
}
