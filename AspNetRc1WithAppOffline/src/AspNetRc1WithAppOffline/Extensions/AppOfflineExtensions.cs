using AspNetRc1WithAppOffline.Middlewares;
using Microsoft.AspNet.Builder;

namespace AspNetRc1WithAppOffline.Extensions
{
    public static class AppOfflineExtensions
    {
        public static IApplicationBuilder UseAppOffline(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AppOfflineMiddleware>();
        }
    }
}
