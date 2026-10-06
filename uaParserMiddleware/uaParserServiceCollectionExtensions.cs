using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware;

/// <summary>Registers uaParser.Net with dependency injection.</summary>
public static class uaParserServiceCollectionExtensions
{
    /// <summary>
    /// Makes <see cref="ClientInfo"/> injectable. It is scoped: every request gets the result for
    /// its own User-Agent header (and client hints, see <see cref="UAParserOptions.UseClientHints"/>).
    /// Outside a request (no HttpContext) all values are null.
    /// Repeated user agents are served from a shared <see cref="ClientInfoCache"/> unless
    /// <see cref="UAParserOptions.CacheCapacity"/> is 0. Calling it again replaces the earlier
    /// settings: the last call wins.
    /// </summary>
    /// <remarks>
    /// Do not inject <see cref="ClientInfo"/> into interactive Blazor components or SignalR hubs:
    /// there it depends on the SignalR transport (the User-Agent of the WebSocket handshake, or all
    /// null with long polling or Azure SignalR Service). Read the User-Agent while the page is
    /// prerendered, or in the browser through JavaScript interop, instead.
    /// </remarks>
    /// <param name="services">The application's services.</param>
    /// <param name="configure">Optional: changes the <see cref="UAParserOptions"/>.</param>
    public static IServiceCollection AddUAParser(this IServiceCollection services, Action<UAParserOptions>? configure = null)
    {
        var options = new UAParserOptions();
        configure?.Invoke(options);
        ArgumentOutOfRangeException.ThrowIfNegative(options.CacheCapacity, nameof(options.CacheCapacity));

        // The last call wins, so a later AddUAParser(o => o.CacheCapacity = 0) really turns the cache off.
        services.RemoveAll<ClientInfoCache>();
        if (options.CacheCapacity > 0)
            services.AddSingleton(new ClientInfoCache(options.CacheCapacity));
        services.RemoveAll<UAParserSettings>();
        services.AddSingleton(new UAParserSettings(options.UseClientHints, options.RequestClientHints));

        services.AddHttpContextAccessor();
        services.TryAddScoped(provider =>
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext is { } context
                ? context.GetClientInfo()
                : UAParser.GetClientInfo(null));

        return services;
    }
}
