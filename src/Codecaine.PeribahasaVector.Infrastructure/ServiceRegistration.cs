using Codecaine.Common.Abstractions;
using Codecaine.Common.AiServices.Interfaces;
using Codecaine.Common.AiServices.Ollama;
using Codecaine.Common.AiServices.OpenAi;
using Codecaine.Common.AspNetCore.Middleware;
using Codecaine.Common.Authentication;
using Codecaine.Common.Authentication.Providers.Services;
using Codecaine.Common.Correlation;
using Codecaine.Common.Date;
using Codecaine.Common.EventConsumer;
using Codecaine.Common.Extensions;
using Codecaine.Common.HttpServices;
using Codecaine.Common.Messaging;
using Codecaine.Common.Messaging.MassTransit;
using Codecaine.Common.Persistence.Dapper;
using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.PeribahasaVector.Domain.Repositories;
using Codecaine.PeribahasaVector.Infrastructure.DataAccess.Repositories;
using Codecaine.PeribahasaVector.Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Codecaine.PeribahasaVector.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Persistence Dapper - NpgSql (Postgres)
            string dapperConnectionString = Environment.GetEnvironmentVariable("ConnectionString__DapperDataBase") ?? "";
            services.AddScoped<IDbConnection>(sp =>
            {
                var conn = new NpgsqlConnection(dapperConnectionString);
                conn.Open(); // Ensure it’s opened before using
                return conn;
            });
            services.AddScoped<DapperDbContext>(); // Register the implementation once
            services.AddScoped<IDapperDbContext>(sp => sp.GetRequiredService<DapperDbContext>());
            services.AddScoped<IDapperUnitOfWork>(sp => sp.GetRequiredService<DapperDbContext>());

            // Register repositories
            services.AddScoped<IPeribahasaRepository, PeribahasaRepository>();

            // MassTransit Publisher
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            services.AddMassTransitRabbitMq();

            // Common Library
            services.AddCommonLibrary();

            // Open AI - Embedding  
            services.AddOptions<OllamaSetting>().BindConfiguration(OllamaSetting.DefaultSectionName);
            services.AddScoped<IEmbeddingService, OllamaEmbeddingService>();

            return services;
        }

        public static IServiceCollection AddMassTransitRabbitMq(this IServiceCollection services)
        {
            string host = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "";
            string userName = Environment.GetEnvironmentVariable("RabbitMq__UserName") ?? "";
            string password = Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "";
            string defaultQueueName = Environment.GetEnvironmentVariable("RabbitMq__DefaultQueueName") ?? "";


            services.AddMassTransit(x =>
            {
                x.AddConsumer<CodecaineMessageConsumer>(); // Register Consumer


                // Add multiple consumers if needed

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, "/", h =>
                    {
                        h.Username(userName);
                        h.Password(password);
                    });

                    cfg.UseRawJsonSerializer();

                    cfg.ReceiveEndpoint(defaultQueueName, e =>
                    {
                        e.ConfigureConsumer<CodecaineMessageConsumer>(context);
                    });

                    // Configure other consumers here

                    cfg.ConfigureEndpoints(context);
                });


            });

            return services;
        }

        public static IServiceCollection AddCommonLibrary(this IServiceCollection services)
        {
            services.AddCompression();
            services.AddHttpClientWithPolicy();

            services.AddTransient<IDateTime, MachineDateTime>();

            // Event consumer
            services.AddScoped<IIntegrationEventConsumer, IntegrationEventConsumer>();

            // HttpService
            services.AddScoped<IHttpService, HttpService>();

            // Authentication - Jwt Service
             services.AddScoped<IJwtService, JwtService>();

            // CorrelationId
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }

        public static IApplicationBuilder UseCommonLibraryBuilder(this IApplicationBuilder app)
        {
            app.UseCompression();
            app.UseCodecaineCommonExceptionHandler();
            app.UseCodecaineCorrelationIdMiddlewareHandler();

            return app;
        }
    }
}
