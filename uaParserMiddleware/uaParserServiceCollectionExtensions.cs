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
    /// </summary>
    public static IServiceCollection AddUAParser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped(provider =>
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext is { } context
                ? context.GetClientInfo()
                : UAParser.GetClientInfo(null));

        return services;
    }
}
