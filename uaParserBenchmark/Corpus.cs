using System.Text.Json;

namespace uaParserBenchmark;

// The user agent sets to measure, read from the files tools/RuleGenerator uses.
public static class Corpus
{
    public const string Legacy = "1.x samples";
    public const string Recent = "recent";
    public const string Upstream = "ua-parser-js tests";

    public static readonly string[] Names = [Legacy, Recent, Upstream];

    private static readonly string Folder = Path.Combine(AppContext.BaseDirectory, "Corpus");

    public static string[] Load(string name) => name switch
    {
        Legacy => Lines("legacy-port-user-agents.txt"),
        Recent => Lines("extra-user-agents.txt"),
        Upstream => UpstreamUserAgents(),
        _ => throw new ArgumentException($"Unknown corpus \"{name}\".", nameof(name)),
    };

    // Exactly `count` user agents, repeating the set when it is smaller, so every benchmark
    // parses the same number of user agents per operation.
    public static string[] Sample(string name, int count)
    {
        var all = Load(name);
        return Enumerable.Range(0, count).Select(i => all[i % all.Length]).ToArray();
    }

    private static string[] Lines(string file) =>
        File.ReadAllLines(Path.Combine(Folder, file))
            .Where(line => line.Trim().Length > 0 && !line.TrimStart().StartsWith('#'))
            .ToArray();

    private static string[] UpstreamUserAgents() =>
        Directory.GetFiles(Path.Combine(Folder, "ua-parser-js"), "*-test.json")
            .Order(StringComparer.Ordinal)
            .SelectMany(file => JsonDocument.Parse(File.ReadAllText(file)).RootElement.EnumerateArray())
            .Select(c => c.TryGetProperty("ua", out var ua) ? ua.GetString() : null)
            .OfType<string>()
            .Distinct()
            .ToArray();
}
