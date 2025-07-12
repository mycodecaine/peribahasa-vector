
using Codecaine.PeribahasaVector.Application;
using Codecaine.PeribahasaVector.Infrastructure;
using Scalar.AspNetCore;

namespace Codecaine.PeribahasaVector.Presentation.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Add Application
            builder.Services.AddApplication();
            // Add Infrastructure
            builder.Services.AddInfrastructure();
            // Version
            builder.Services.AddApiVersioning();
            // HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "Codecaine Peribahasa Vector API";
                    options.Theme = ScalarTheme.Default;

                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCommonLibraryBuilder();

            app.MapControllers();

            app.Run();
        }
    }
}
