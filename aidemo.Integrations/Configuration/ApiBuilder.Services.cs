using Microsoft.Extensions.DependencyInjection;
using System;

namespace aidemo.Integrations.Configuration
{
    public static partial class ApiBuilder
    {
        public static void AddExternalServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            //services.AddHttpClient();
        }
    }
}
