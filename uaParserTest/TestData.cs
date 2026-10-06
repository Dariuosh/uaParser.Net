using System.Text.Json;

namespace uaParserTest;

// Loads the test data files copied next to the test assembly.
internal static class TestData
{
    private static readonly string Folder = Path.Combine(AppContext.BaseDirectory, "TestData");

    // What the rules give for every corpus user agent (tools/RuleGenerator: expected-results.json).
    public static readonly IReadOnlyDictionary<string, JsonElement> Golden = LoadGolden();

    // ua-parser-js test cases whose result uaParser.Net changes on purpose: (category, desc, field) -> value.
    public static readonly IReadOnlyDictionary<(string Category, string Desc, string Field), string?> UpstreamDifferences = LoadDifferences();

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, JsonElement[]> UpstreamFiles = new();

    public static JsonElement[] Upstream(string category) =>
        UpstreamFiles.GetOrAdd(category, c =>
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(Folder, "ua-parser-js", $"{c}-test.json")));
            return doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToArray();
        });

    // A JSON value as uaParser.Net reports it: null and ua-parser-js's "undefined" are both null.
    public static string? Value(JsonElement parent, string name) =>
        parent.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String && value.GetString() != "undefined"
            ? value.GetString()
            : null;

    private static Dictionary<string, JsonElement> LoadGolden()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(Folder, "expected-results.json")));
        return doc.RootElement.GetProperty("cases").EnumerateArray()
            .ToDictionary(c => c.GetProperty("ua").GetString()!, c => c.Clone());
    }

    private static Dictionary<(string, string, string), string?> LoadDifferences()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(Folder, "upstream-test-differences.json")));
        return doc.RootElement.EnumerateArray().ToDictionary(
            d => (d.GetProperty("category").GetString()!, d.GetProperty("desc").GetString()!, d.GetProperty("field").GetString()!),
            d => d.GetProperty("value").GetString());
    }
}
