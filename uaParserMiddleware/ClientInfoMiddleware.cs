using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware
{
    public class ClientInfoMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly Options _options;

        public ClientInfoMiddleware(RequestDelegate next, Options options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context, ClientInfo clientInfo)
        {
            var request = context.Request;

            await _next(context);
            string uaString = request.Headers["User-Agent"].ToString();
            if (_options.BrowserParser) clientInfo.Browser = UAParser.GetBrowser(uaString);
            if (_options.CPUParser) clientInfo.CPU = UAParser.GetCPU(uaString);
            if (_options.EngineParser) clientInfo.Engine = UAParser.GetEngine(uaString);
            if (_options.OSParser) clientInfo.OS = UAParser.GetOS(uaString);
            if (_options.DeviceParser) clientInfo.Device = UAParser.GetDevice(uaString);
        }
    }

    public static class UserAgentMiddlewareExtensions
    {
        public static IApplicationBuilder UseUAParser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientInfoMiddleware>();
        }
    }
}