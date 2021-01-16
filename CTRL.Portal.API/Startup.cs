
using CTRL.Portal.API.Configuration;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Middleware;
using CTRL.Portal.API.Services;
using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Repositories;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;

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

            var authConfig = new AuthenticationConfiguration();
            var appSettingsAuthConfig = Configuration.GetSection("JWT");
            appSettingsAuthConfig.Bind(authConfig);
            services.AddSingleton(authConfig);

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = authConfig.ValidAudience,
                    ValidIssuer = authConfig.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Secret))
                };
            });

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IUserSettingsService, UserSettingsService>();
            services.AddSingleton<IUserSettingsRepository, UserSettingsRepository>();

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("StandardPolicy");

            app.UseAuthorization();

            app.UseMiddleware(typeof(ApiMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            runner.MigrateUp();
        }
    }
}
