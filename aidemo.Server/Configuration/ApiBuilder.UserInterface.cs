namespace aidemo.Server.Configuration
{
    public static partial class ApiBuilder
    {
        // Serves the React build in wwwroot same-origin with the API. There is no SPA fallback: a path with no endpoint and no file is a 404.
        public static void UseUserInterface(this WebApplication app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
