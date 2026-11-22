using Microsoft.AspNetCore.Builder;

namespace aidemo.Endpoints.Configuration
{
    public static partial class ApiBuilder
    {
        public static void MapAllEndpoints(this WebApplication app)
        {
            app.MapSampleEndpoints();
        }
    }
}
