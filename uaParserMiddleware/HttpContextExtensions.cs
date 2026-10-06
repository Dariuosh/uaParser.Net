using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware;

/// <summary>Reads the client information of a request.</summary>
public static class HttpContextExtensions
{
    private static readonly object ItemKey = new();

    /// <summary>
    /// The client information for this request's User-Agent header (the first one, if a client sent
    /// several, as Node.js and ua-parser-js do), improved with the request's client hints (see
    /// <see cref="ClientHints"/>) unless <see cref="UAParserOptions.UseClientHints"/> is off. It is
    /// worked out the first time it is asked for and kept on this request only, so requests never
    /// share results.
    /// When <see cref="uaParserServiceCollectionExtensions.AddUAParser"/> registered a
    /// <see cref="ClientInfoCache"/>, a user agent seen before is not parsed again.
    /// </summary>
    /// <param name="context">The request.</param>
    public static ClientInfo GetClientInfo(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Items.TryGetValue(ItemKey, out var stored) && stored is ClientInfo info)
            return info;

        var headers = context.Request.Headers;
        // StringValues.ToString() would join several headers with commas into a string nobody sent.
        var userAgent = headers.UserAgent is { Count: > 0 } values ? values[0] : null;
        var services = context.RequestServices;
        var hints = services?.GetService<UAParserSettings>()?.UseClientHints ?? true
            ? ClientHints.FromHeaders(name => headers.TryGetValue(name, out var value) && value.Count > 0 ? string.Join(", ", value.ToArray()) : null)
            : null;
        info = services?.GetService<ClientInfoCache>() is { } cache
            ? cache.GetClientInfo(userAgent, hints)
            : UAParser.GetClientInfo(userAgent, hints);

        context.Items[ItemKey] = info;
        return info;
    }
}
