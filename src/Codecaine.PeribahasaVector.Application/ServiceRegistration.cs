using Codecaine.Common.Behaviours;
using Codecaine.PeribahasaVector.Application.Mappers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Codecaine.PeribahasaVector.Application
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Dependency injection for Application layer
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            // Auto mapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }

    }
}
