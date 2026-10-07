namespace uaParserLibrary.Models;

/// <summary>Everything uaParser.Net reads from one user agent.</summary>
/// <param name="UserAgent">The user agent that was parsed (at most 500 characters, as in ua-parser-js).</param>
/// <param name="Browser">The browser.</param>
/// <param name="CPU">The processor architecture.</param>
/// <param name="Device">The device.</param>
/// <param name="Engine">The browser engine.</param>
/// <param name="OS">The operating system.</param>
/// <param name="GPU">The graphics card, when a WebGL renderer string was given; otherwise <see langword="null"/>.</param>
public sealed record ClientInfo(string UserAgent, Browser Browser, CPU CPU, Device Device, Engine Engine, OS OS, GPU? GPU = null)
{
    /// <summary>The bot the user agent belongs to; <see cref="Bot.None"/> when it is not a bot.</summary>
    public Bot Bot { get; init; } = Bot.None;

    /// <summary>The User-Agent Client Hints that were used, or <see langword="null"/> when none were given.</summary>
    public ClientHints? Hints { get; init; }
}
