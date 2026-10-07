using uaParserLibrary.Models;

namespace uaParserDemoComponents.Results;

/// <summary>Results in plain words, for headlines and lists.</summary>
public static class Words
{
    /// <summary>"Samsung Internet 28", or null when the browser is unknown.</summary>
    public static string? Browser(Browser browser) => Join(browser.Name, browser.Major ?? browser.Version);

    /// <summary>"Android 15", or null when the operating system is unknown.</summary>
    public static string? OS(OS os) => Join(os.Name, os.Version);

    /// <summary>"Blink 130.0.0.0", or null.</summary>
    public static string? Engine(Engine engine) => Join(engine.Name, engine.Version);

    /// <summary>"Samsung SM-S931B", or null when the user agent names no device.</summary>
    public static string? DeviceName(Device device) => Join(device.Vendor, device.Model);

    /// <summary>"phone", "tablet", ... for a device type; null when there is none.</summary>
    public static string? DeviceKind(string? type) => type switch
    {
        null => null,
        DeviceTypes.Mobile => "phone",
        DeviceTypes.Tablet => "tablet",
        DeviceTypes.SmartTV => "smart TV",
        DeviceTypes.Console => "game console",
        DeviceTypes.Wearable => "wearable",
        DeviceTypes.Embedded => "embedded device",
        _ => type,
    };

    /// <summary>"search engine crawler", "AI crawler", ... for a bot category; "bot" when there is none.</summary>
    public static string BotKind(string? category) => category switch
    {
        BotCategories.SearchEngine => "search engine crawler",
        BotCategories.AiCrawler => "AI crawler or assistant",
        BotCategories.Seo => "SEO tool",
        BotCategories.Monitoring => "monitoring service",
        BotCategories.Scanner => "security scanner",
        BotCategories.Advertising => "advertising crawler",
        BotCategories.SocialPreview => "link preview",
        BotCategories.FeedReader => "feed reader",
        BotCategories.HttpLibrary => "HTTP library or tool",
        BotCategories.Archiver => "web archiver",
        BotCategories.Academic => "research crawler",
        BotCategories.BrowserAutomation => "automated browser",
        null => "bot",
        _ => category,
    };

    /// <summary>"a link preview", "an AI crawler", "an SEO tool".</summary>
    public static string WithArticle(string noun) =>
        (noun.Length > 0 && "aeiouAEIOU".Contains(noun[0])) || noun.StartsWith("SEO") || noun.StartsWith("HTTP")
            ? "an " + noun
            : "a " + noun;

    /// <summary>A one-line summary, such as "Samsung Internet 28 · Android 15 · Samsung SM-S931B".</summary>
    public static string Summary(ClientInfo info)
    {
        if (info.Bot.IsBot)
            return $"{info.Bot.Name} ({BotKind(info.Bot.Category)})";

        var parts = new[] { Browser(info.Browser), OS(info.OS), DeviceName(info.Device) ?? DeviceKind(info.Device.Type) }
            .Where(p => p is not null)
            .ToArray();

        return parts.Length == 0 ? "Nothing recognised" : string.Join(" · ", parts);
    }

    /// <summary>Joins the non-empty values with a space; null when there are none.</summary>
    public static string? Join(params string?[] values)
    {
        var present = values.Where(v => !string.IsNullOrEmpty(v)).ToArray();
        return present.Length == 0 ? null : string.Join(' ', present);
    }
}
