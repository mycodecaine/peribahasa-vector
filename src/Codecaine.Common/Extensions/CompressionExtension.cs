﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Codecaine.Common.Extensions
{
    public static class CompressionExtension
    {
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            return services;
        }

        public static IApplicationBuilder UseCompression(this IApplicationBuilder app)
        {
            return app.UseResponseCompression();
        }
    }
}
