using uaParserLibrary;

namespace uaParserMiddleware;

/// <summary>Options for <see cref="uaParserServiceCollectionExtensions.AddUAParser"/>.</summary>
public class UAParserOptions
{
    /// <summary>
    /// How many user agents the shared <see cref="ClientInfoCache"/> keeps per generation.
    /// 0 turns the cache off. Default: 1024.
    /// </summary>
    public int CacheCapacity { get; set; } = 1024;
}
