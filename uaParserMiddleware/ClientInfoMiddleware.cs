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

    /// <summary>Parses the request's User-Agent header, then runs the rest of the pipeline.</summary>
    /// <param name="context">The request.</param>
    public Task InvokeAsync(HttpContext context)
    {
        context.GetClientInfo();
        return _next(context);
    }
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
