using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace uaParserMiddleware;

/// <summary>
/// Parses the User-Agent header before the rest of the pipeline runs. Optional:
/// <see cref="HttpContextExtensions.GetClientInfo"/> parses on first use without it.
/// </summary>
public class ClientInfoMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>Creates the middleware.</summary>
    /// <param name="next">The rest of the pipeline.</param>
    public ClientInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Parses the request's User-Agent header, then runs the rest of the pipeline. With
    /// <see cref="UAParserOptions.RequestClientHints"/> it also asks for the high-entropy client hints.
    /// </summary>
    /// <param name="context">The request.</param>
    public Task InvokeAsync(HttpContext context)
    {
        if (context.RequestServices?.GetService(typeof(UAParserSettings)) is UAParserSettings { RequestClientHints: true })
            context.Response.Headers.Append("Accept-CH", AcceptClientHints);

        context.GetClientInfo();
        return _next(context);
    }

    private static readonly string AcceptClientHints = string.Join(", ", uaParserLibrary.Models.ClientHints.HighEntropyHeaders);
}

/// <summary>Adds <see cref="ClientInfoMiddleware"/> to a pipeline.</summary>
public static class UserAgentMiddlewareExtensions
{
    /// <summary>
    /// Parses the User-Agent header of every request before the rest of the pipeline runs. Optional:
    /// <see cref="HttpContextExtensions.GetClientInfo"/> and an injected ClientInfo parse on first use without it.
    /// </summary>
    /// <param name="builder">The application's pipeline.</param>
    public static IApplicationBuilder UseUAParser(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClientInfoMiddleware>();
    }
}
