using Codecaine.Common.Telemetry.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;

namespace Codecaine.Common.Telemetry
{
    public static class TelemetryServiceRegistration
    {
        public static void AddTelemetryRegistration(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(Logger.ConfigureLogger);

            var tracingSource = Environment.GetEnvironmentVariable("Telemetry__TracingSource");
            var tracingOtlpEndPoint = Environment.GetEnvironmentVariable("Telemetry__OtlpEndPoint");
            var metricsMeter = Environment.GetEnvironmentVariable("Telemetry__Metrics");



            builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation() // Captures HTTP requests
                    .AddHttpClientInstrumentation() // Captures outgoing HTTP requests
                    .AddSource(tracingSource)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(tracingSource))
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(tracingOtlpEndPoint); // OpenTelemetry Collector OTLP gRPC port
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                    })                    
                    .AddConsoleExporter(); // Debugging traces in console
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation() // Collects HTTP metrics
                    .AddRuntimeInstrumentation() // Collects GC, CPU, etc.
                    .AddMeter(metricsMeter)
                    .AddPrometheusExporter(); // Exposes metrics for Prometheus
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddOpenTelemetry(loggingBuilder =>
            {
                loggingBuilder.IncludeFormattedMessage = true;
                loggingBuilder.IncludeScopes = true;
                loggingBuilder.ParseStateValues = true;
            });
        }

        public static IApplicationBuilder UseTelemetryRegistration(this WebApplication app)
        {
            // Enable Serilog request logging
            app.UseSerilogRequestLogging();

            app.UseMetricServer();

            app.MapPrometheusScrapingEndpoint();

            return app;
        }
    }
}
