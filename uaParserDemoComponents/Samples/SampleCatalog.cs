using uaParserLibrary;
using uaParserLibrary.Models;
using uaParserSamples;

using uaParserDemoComponents.Results;

namespace uaParserDemoComponents.Samples;

/// <summary>An example user agent and what uaParser.Net reads from it.</summary>
public sealed record SampleEntry(string Group, string UserAgent, ClientInfo Info)
{
    public string Title => Words.Browser(Info.Browser) ?? "Unknown browser";

    public string Subtitle =>
        string.Join(" · ", new[] { Words.OS(Info.OS), Words.DeviceName(Info.Device) }.Where(p => p is not null)) is { Length: > 0 } text
            ? text
            : "No operating system";

    public bool Matches(string filter) =>
        UserAgent.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || Subtitle.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || Group.Contains(filter, StringComparison.OrdinalIgnoreCase)
        || (Words.DeviceKind(Info.Device.Type)?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false);
}

/// <summary>The example user agents of tools/RuleGenerator/corpus/extra-user-agents.txt, parsed once.</summary>
public static class SampleCatalog
{
    private static readonly Lazy<IReadOnlyList<SampleEntry>> Entries = new(() =>
        [.. SampleUserAgents.All.Select(s => new SampleEntry(s.Group, s.UserAgent, UAParser.GetClientInfo(s.UserAgent)))]);

    public static IReadOnlyList<SampleEntry> All => Entries.Value;

    public static IReadOnlyList<string> Groups { get; } = [.. SampleUserAgents.All.Select(s => s.Group).Distinct()];
}
