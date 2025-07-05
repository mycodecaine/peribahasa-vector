using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

namespace Codecaine.Common.Telemetry.Logging
{
    public static  class Logger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
        (context, loggerConfiguration) =>
        {
            var env = context.HostingEnvironment;
            
            var grafanalokiUrl = Environment.GetEnvironmentVariable("Telemetry__GrafanaUrl"); 
            var jobName = Environment.GetEnvironmentVariable("Telemetry__JobName") ?? "Codecaine.Api";
            var presentation = Environment.GetEnvironmentVariable("Telemetry__Presentation");
            loggerConfiguration.MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.GrafanaLoki(grafanalokiUrl,
                    [
                        new() { Key = "job", Value = jobName } // Set a custom job name
                        ]
                );// Send logs to Loki

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Override(presentation, LogEventLevel.Information);
            }


        };
    }
}
