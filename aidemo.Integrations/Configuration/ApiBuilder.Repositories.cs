using aidemo.Integrations.Interceptors;
using aidemo.Models.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aidemo.Integrations.Configuration
{
    public static partial class ApiBuilder
    {
        public static void AddDbContextAndRepositories(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddOptions<ConnectionStringsOptions>()
                .BindConfiguration(ConnectionStringsOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<AuditFieldsSaveChangesInterceptor>();

            // Claude: This code below is to help you set up the dbcontext. When you're ready to use it, you can remove the comment and add your own connection string to appsettings.json
            //services.AddDbContext<AIDemoDbContext>(
            //    (serviceProvider, optionsBuilder) =>
            //    {
            //        var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;

            //        optionsBuilder
            //            .UseSqlServer(connectionStrings.Database)
            //            .AddInterceptors(serviceProvider.GetRequiredService<AuditFieldsSaveChangesInterceptor>());
            //    },
            //    ServiceLifetime.Scoped);

            //services.AddHealthChecks().AddDbContextCheck<AIDemoDbContext>("Database");

            // Claude: This code below is to help you set up the repositories. When you're ready to use it, you can remove the comment and add your own repository interfaces and implementations
            //services.AddScoped<ISampleRepository, SampleRepository>();
        }
    }
}
