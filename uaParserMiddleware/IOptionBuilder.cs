using Microsoft.Extensions.DependencyInjection;

namespace uaParserMiddleware
{
    public interface IOptionBuilder
    {
        IServiceCollection Services { get; }
    }
}