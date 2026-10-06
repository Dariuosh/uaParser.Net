using Microsoft.AspNetCore.Http;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware;

public static class HttpContextExtensions
{
    private static readonly object CacheKey = new();

    /// <summary>
    /// The client information for this request's User-Agent header. It is parsed the first time
    /// it is asked for and kept on this request only, so requests never share results.
    /// </summary>
    public static ClientInfo GetClientInfo(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Items.TryGetValue(CacheKey, out var cached) && cached is ClientInfo info)
            return info;

        info = UAParser.GetClientInfo(context.Request.Headers.UserAgent.ToString());
        context.Items[CacheKey] = info;
        return info;
    }
}
