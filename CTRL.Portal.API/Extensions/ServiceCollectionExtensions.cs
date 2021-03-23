using CTRL.Authentication;
using CTRL.Authentication.Configuration;
using CTRL.Authentication.Implementation;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Migrations;
using CTRL.Portal.Services.Configuration;
using CTRL.Portal.Services.Implementation;
using CTRL.Portal.Services.Interfaces;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTRL.Portal.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            var spaUrl = configuration.GetValue<string>("ServiceUrls:Spa");

            services.AddTransient<IAuthenticationTokenManager, AuthenticationTokenManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>(sp => new AuthenticationService(
                spaUrl, 
                sp.GetRequiredService<UserManager<ApplicationUser>>(), 
                sp.GetRequiredService<ICodeService>(),
                sp.GetRequiredService<IEmailProvider>(), 
                sp.GetRequiredService<IAuthenticationTokenManager>(), 
                sp.GetRequiredService<IBusinessEntityService>(), 
                sp.GetRequiredService<IUserSettingsService>()));

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IBusinessEntityService, BusinessEntityService>(sp => new BusinessEntityService(
                sp.GetRequiredService<IBusinessEntityRepository>(), 
                sp.GetRequiredService<ICodeService>(),
                sp.GetRequiredService<IEmailProvider>(),
                sp.GetRequiredService<IBusinessEntityCodeRepository>(),
                spaUrl));

            services.AddSingleton<IBusinessEntityRepository, BusinessEntityRepository>();
            services.AddSingleton<IUserSettingsService, UserSettingsService>();
            services.AddSingleton<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IEmailProvider, EmailProvider>();
            services.AddSingleton<IUtilityManager, UtilityManager>();
            services.AddSingleton<ICodeRepository, CodeRepository>();
            services.AddSingleton<ICodeService, CodeService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IBusinessEntityCodeRepository, BusinessEntityCodeRepository>();

            return services;
        }

        public static IServiceCollection AddFluentMigrator(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(runner => runner.AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(MigrationEngine.GetCustomMigrationAssemblies()).For.Migrations());

            return services;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
        {
            BindConfiguration<EmailConfiguration>(services, config, "EmailConfiguration");
            BindConfiguration<AuthenticationConfiguration>(services, config, "JWT");
            BindConfiguration<CodeConfiguration>(services, config, "CodeConfiguration");

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            var schemeName = "ApiAuthentication";

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = schemeName;
                    options.DefaultChallengeScheme = schemeName;
                    options.DefaultSignInScheme = schemeName;
                })
                .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(schemeName, "Api Authenticaiton", null);

            return services;
        }

        private static void BindConfiguration<TConfigClass>(IServiceCollection services, IConfiguration config, string appSettingsSection)
            where TConfigClass : class, new()
        {
            var configurationInstance = new TConfigClass();
            var appSettingsObject = config.GetSection(appSettingsSection);

            appSettingsObject.Bind(configurationInstance);
            services.AddSingleton(configurationInstance);
        }
    }
}
