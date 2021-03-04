using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Configuration;
using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Middleware;
using CTRL.Portal.API.Services;
using CTRL.Portal.Data.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CTRL.Portal.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationTokenManager, AuthenticationTokenManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IUserSettingsService, UserSettingsService>();
            services.AddSingleton<IUserSettingsRepository, UserSettingsRepository>();
            services.AddSingleton<IEmailProvider, EmailProvider>();
            services.AddSingleton<IUtilityManager, UtilityManager>();
            services.AddSingleton<ICodeRepository, CodeRepository>();
            services.AddSingleton<ICodeService, CodeService>();

            return services;
        }

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

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
        {
            BindConfiguration<EmailConfiguration>(services, config, "EmailConfiguration");
            BindConfiguration<AuthenticationConfiguration>(services, config, "JWT");
            BindConfiguration<CodeConfiguration>(services, config, "CodeConfiguration");

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiNames.ApiAuthenticationScheme;
                    options.DefaultChallengeScheme = ApiNames.ApiAuthenticationScheme;
                    options.DefaultSignInScheme = ApiNames.ApiAuthenticationScheme;
                })
                .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(ApiNames.ApiAuthenticationScheme, "Api Authenticaiton", null);

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
