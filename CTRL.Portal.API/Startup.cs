using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Extensions;
using CTRL.Portal.API.Middleware;
using CTRL.Portal.Data.Configuration;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
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
            services.AddCustomServices();

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
