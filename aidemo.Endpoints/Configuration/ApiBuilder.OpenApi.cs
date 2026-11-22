using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace aidemo.Endpoints.Configuration
{
    public static partial class ApiBuilder
    {
        public static void AddOpenApiServices(this IServiceCollection services)
        {
            services.AddOpenApi();
        }

        // Development only: other environments expose neither the document nor the reference UI.
        public static void UseOpenApiReference(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                return;

            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}
