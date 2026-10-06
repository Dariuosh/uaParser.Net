using System.Globalization;

using uaParserSamples;

namespace uaParserDemoComponents.AccessLogs;

/// <summary>
/// A made-up nginx access log built from the example user agents, the same every time. The IP
/// addresses come from the ranges reserved for documentation (RFC 5737).
/// </summary>
public static class ExampleLog
{
    public const int DefaultLines = 5000;

    // How often each example group appears, roughly like the traffic of a public website.
    private static readonly Dictionary<string, double> GroupWeights = new()
    {
        ["Desktop browsers"] = 34,
        ["iOS and iPadOS"] = 22,
        ["Android"] = 26,
        ["In-app browsers and WebViews"] = 7,
        ["TVs, consoles, XR and other devices"] = 2,
        ["Bots, crawlers and tools"] = 8,
        ["Rare devices and systems"] = 1,
    };

    private static readonly string[] Networks = ["192.0.2", "198.51.100", "203.0.113"];

    private static readonly string[] Paths =
    [
        "/", "/", "/", "/products", "/products/42", "/blog/reading-user-agents", "/search?q=phone",
        "/api/items?page=2", "/favicon.ico", "/images/logo.svg", "/about", "/robots.txt",
    ];

    public static IEnumerable<string> Lines(int count = DefaultLines, int seed = 2026)
    {
        var random = new Random(seed);
        var groups = SampleUserAgents.All.GroupBy(s => s.Group).Select(g => (Group: g.Key, Agents: g.ToArray())).ToArray();
        var weights = groups.Select(g => GroupWeights.GetValueOrDefault(g.Group, 1)).ToArray();
        var totalWeight = weights.Sum();

        var time = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);

        for (var i = 0; i < count; i++)
        {
            // A group by weight, then a user agent of it, favouring the first ones (the common ones).
            var pick = random.NextDouble() * totalWeight;
            var g = 0;
            while (g < weights.Length - 1 && (pick -= weights[g]) > 0)
                g++;

            var agents = groups[g].Agents;
            var agent = agents[(int)(Math.Pow(random.NextDouble(), 1.7) * agents.Length)].UserAgent;

            time = time.AddSeconds(random.Next(1, 40));
            var ip = $"{Networks[random.Next(Networks.Length)]}.{random.Next(1, 255)}";
            var path = Paths[random.Next(Paths.Length)];
            var status = random.Next(100) switch { < 86 => 200, < 93 => 304, < 98 => 404, _ => 301 };
            var bytes = status == 200 ? random.Next(400, 48_000) : 0;
            var referer = random.Next(4) == 0 ? "https://example.com/" : "-";

            yield return string.Create(CultureInfo.InvariantCulture,
                $"{ip} - - [{time:dd/MMM/yyyy:HH:mm:ss} +0000] \"GET {path} HTTP/1.1\" {status} {bytes} \"{referer}\" \"{agent.Replace("\"", "\\\"")}\"");
        }
    }
}
