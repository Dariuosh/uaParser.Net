namespace uaParserBlazorServerDemo;

/// <summary>
/// User-Agent Client Hints: request headers that Chromium-based browsers send besides the
/// User-Agent header. uaParser.Net 2.0 does not read them; the home page shows them next to it.
/// </summary>
public static class ClientHintHeaders
{
    /// <summary>Sent to every secure site (https, or localhost) without being asked.</summary>
    public static readonly string[] LowEntropy = ["Sec-CH-UA", "Sec-CH-UA-Mobile", "Sec-CH-UA-Platform"];

    /// <summary>Sent only to sites that ask for them with Accept-CH.</summary>
    public static readonly string[] HighEntropy =
    [
        "Sec-CH-UA-Platform-Version", "Sec-CH-UA-Full-Version-List", "Sec-CH-UA-Model",
        "Sec-CH-UA-Arch", "Sec-CH-UA-Bitness", "Sec-CH-UA-Form-Factors",
    ];

    /// <summary>Asks Chromium-based browsers for the high-entropy hints on later requests.</summary>
    public static IApplicationBuilder UseClientHints(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            context.Response.Headers["Accept-CH"] = string.Join(", ", HighEntropy);

            // On the home page, ask for a retry with these hints at once, so even a first visit shows them.
            if (context.Request.Path == "/")
                context.Response.Headers["Critical-CH"] = "Sec-CH-UA-Platform-Version, Sec-CH-UA-Full-Version-List";

            return next(context);
        });
}
