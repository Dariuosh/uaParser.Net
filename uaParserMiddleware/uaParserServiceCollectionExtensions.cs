using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;

using uaParserLibrary.Models;

namespace uaParserMiddleware
{
    public static class uaParserServiceCollectionExtensions
    {
        public static IServiceCollection AddUAParser(this IServiceCollection services, Action<Options> configureOption)
        {
            services.Configure(configureOption);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Options>>().Value);
            services.AddSingleton<ClientInfo>();

            return services;
        }
    }
}