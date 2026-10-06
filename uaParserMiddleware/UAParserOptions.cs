using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserMiddleware;

/// <summary>Options for <see cref="uaParserServiceCollectionExtensions.AddUAParser"/>.</summary>
public class UAParserOptions
{
    /// <summary>
    /// How many user agents the shared <see cref="ClientInfoCache"/> keeps per generation.
    /// 0 turns the cache off. Default: 1024.
    /// </summary>
    public int CacheCapacity { get; set; } = 1024;

    /// <summary>
    /// Whether the request's User-Agent Client Hints (Sec-CH-UA-* headers, see
    /// <see cref="ClientHints"/>) improve the result. Default: <see langword="true"/>.
    /// </summary>
    public bool UseClientHints { get; set; } = true;

    /// <summary>
    /// Whether <see cref="UserAgentMiddlewareExtensions.UseUAParser"/> asks browsers for the
    /// high-entropy client hints (<see cref="ClientHints.HighEntropyHeaders"/>: the full browser
    /// version, the operating system version, the CPU and the device model) with an Accept-CH
    /// response header. Browsers send them from the next request on. Default:
    /// <see langword="false"/>, because these hints tell sites more about the user's device.
    /// </summary>
    /// <remarks>If a response depends on a hint, also add that header to the response's Vary header.</remarks>
    public bool RequestClientHints { get; set; }
}

// The options as AddUAParser registered them (a copy, so later changes to the options object do
// not change them).
internal sealed record UAParserSettings(bool UseClientHints, bool RequestClientHints);
