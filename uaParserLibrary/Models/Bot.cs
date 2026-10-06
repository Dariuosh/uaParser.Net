namespace uaParserLibrary.Models;

/// <summary>
/// The bot a user agent belongs to: a search engine or AI crawler, a link preview, an uptime
/// monitor, a security scanner, an HTTP library and so on. Any program can send any user agent,
/// so this tells you what a client says it is, not what it is.
/// </summary>
/// <param name="Name">For example "Googlebot", "GPTBot" or "curl", or <see langword="null"/> when the user agent is not a bot.</param>
/// <param name="Category">One of the <see cref="BotCategories"/> values, or <see langword="null"/> for a bot that is only known by its name (a word ending in "bot", "crawler", "spider" or "scraper").</param>
/// <param name="Url">A web page about the bot, or <see langword="null"/>.</param>
public sealed record Bot(string? Name, string? Category, string? Url)
{
    /// <summary>The result for a user agent that is not a bot.</summary>
    public static Bot None { get; } = new(null, null, null);

    /// <summary>Whether the user agent belongs to a bot.</summary>
    public bool IsBot => Name is not null;

    /// <summary>A one-line description, for example "Bot    : GPTBot ai-crawler".</summary>
    public override string ToString() => Describe.Line("Bot", Name, Category);
}
