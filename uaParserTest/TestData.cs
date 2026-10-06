using System.Text.Json;

namespace uaParserTest;

// Loads the test data files copied next to the test assembly.
internal static class TestData
{
    private static readonly string Folder = Path.Combine(AppContext.BaseDirectory, "TestData");

    public static readonly IReadOnlyDictionary<string, JsonElement> Golden = LoadGolden();

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
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(Folder, "ua-parser-js.golden.json")));
        return doc.RootElement.GetProperty("cases").EnumerateArray()
            .ToDictionary(c => c.GetProperty("ua").GetString()!, c => c.Clone());
    }
}
