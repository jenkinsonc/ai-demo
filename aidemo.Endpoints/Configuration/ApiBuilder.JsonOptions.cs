using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace aidemo.Endpoints.Configuration
{
    public static partial class ApiBuilder
    {
        public static void ConfigureJsonOptions(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        }
    }
}
