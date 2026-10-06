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

    public ClientInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        context.GetClientInfo();
        return _next(context);
    }
}

public static class UserAgentMiddlewareExtensions
{
    public static IApplicationBuilder UseUAParser(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClientInfoMiddleware>();
    }
}
