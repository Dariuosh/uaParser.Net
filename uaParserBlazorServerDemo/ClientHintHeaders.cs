namespace uaParserBlazorServerDemo;

/// <summary>
/// User-Agent Client Hints for the home page. AddUAParser(o => o.RequestClientHints = true) already
/// asks browsers for the high-entropy hints (Accept-CH), which they send from the next request on.
/// </summary>
public static class ClientHintHeaders
{
    /// <summary>
    /// On the home page, also asks for a retry with the most useful hints at once (Critical-CH), so
    /// even a first visit shows them.
    /// </summary>
    public static IApplicationBuilder UseCriticalClientHints(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            if (context.Request.Path == "/")
                context.Response.Headers["Critical-CH"] = "Sec-CH-UA-Platform-Version, Sec-CH-UA-Full-Version-List";

            return next(context);
        });
}
