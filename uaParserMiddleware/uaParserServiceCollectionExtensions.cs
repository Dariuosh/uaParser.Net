using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware;

public static class uaParserServiceCollectionExtensions
{
    /// <summary>
    /// Makes <see cref="ClientInfo"/> injectable. It is scoped: every request gets the result for
    /// its own User-Agent header. Outside a request (no HttpContext) all values are null.
    /// Repeated user agents are served from a shared <see cref="ClientInfoCache"/> unless
    /// <see cref="UAParserOptions.CacheCapacity"/> is 0.
    /// </summary>
    public static IServiceCollection AddUAParser(this IServiceCollection services, Action<UAParserOptions>? configure = null)
    {
        var options = new UAParserOptions();
        configure?.Invoke(options);
        ArgumentOutOfRangeException.ThrowIfNegative(options.CacheCapacity, nameof(options.CacheCapacity));

        if (options.CacheCapacity > 0)
            services.AddSingleton(new ClientInfoCache(options.CacheCapacity));

        services.AddHttpContextAccessor();
        services.AddScoped(provider =>
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext is { } context
                ? context.GetClientInfo()
                : UAParser.GetClientInfo(null));

        return services;
    }
}
