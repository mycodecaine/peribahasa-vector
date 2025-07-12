using Microsoft.AspNetCore.Builder;

namespace Codecaine.Common.AspNetCore.Middleware
{
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCodecaineCorrelationIdMiddlewareHandler(this IApplicationBuilder builder)
           => builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
