using Codecaine.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using System.Diagnostics;

namespace Codecaine.Common.AspNetCore.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
           
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var activity = Activity.Current ?? new Activity("Incoming Request").Start();

            var correlationIdGenerator = context.RequestServices.GetRequiredService<ICorrelationIdGenerator>();

            var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existing)
                ? existing.ToString()
                : correlationIdGenerator.Set().ToString();

            context.Response.Headers[CorrelationIdHeader] = correlationId;
            activity.SetTag("correlation_id", correlationId);


            await _next(context);

        }
    }
}
