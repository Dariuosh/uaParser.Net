using System.Diagnostics;

using uaParserLibrary;
using uaParserLibrary.Models;

using uaParserDemoComponents.Results;
using uaParserDemoComponents.Shared;

namespace uaParserDemoComponents.AccessLogs;

/// <summary>
/// Counts the user agents of a log, then parses each distinct one once. Parsing is done in small
/// steps (<see cref="ParseSome"/>) so a WebAssembly page can show progress between them.
/// </summary>
public sealed class LogAnalysis
{
    /// <summary>Distinct user agents kept; requests with further new user agents are only counted.</summary>
    public const int MaxDistinct = 100_000;

    private readonly LogLineReader _reader = new();
    private readonly Dictionary<string, long> _counts = new(StringComparer.Ordinal);
    private readonly Dictionary<string, long> _formats = [];
    private KeyValuePair<string, long>[]? _pending;
    private List<LogRow>? _rows;
    private int _parsed;
    private TimeSpan _parseTime;

    public long Lines { get; private set; }

    /// <summary>Lines with a user agent.</summary>
    public long Requests { get; private set; }

    /// <summary>Requests whose user agent was not kept because there were more than <see cref="MaxDistinct"/>.</summary>
    public long Overflow { get; private set; }

    public int Distinct => _counts.Count;

    public int Parsed => _parsed;

    /// <summary>The kind of log most lines looked like.</summary>
    public string? Format => _formats.Count == 0 ? null : _formats.MaxBy(f => f.Value).Key;

    public void AddLine(string line)
    {
        Lines++;
        var userAgent = _reader.Read(line);
        if (_reader.Format is { } format)
            _formats[format] = _formats.GetValueOrDefault(format) + 1;

        if (userAgent is null)
            return;

        Requests++;
        if (_counts.TryGetValue(userAgent, out var count))
            _counts[userAgent] = count + 1;
        else if (_counts.Count < MaxDistinct)
            _counts[userAgent] = 1;
        else
            Overflow++;
    }

    /// <summary>Parses distinct user agents for about <paramref name="budget"/>; true when all are parsed.</summary>
    public bool ParseSome(TimeSpan budget)
    {
        _pending ??= [.. _counts];
        _rows ??= new List<LogRow>(_pending.Length);

        var start = Stopwatch.GetTimestamp();
        while (_parsed < _pending.Length)
        {
            var (userAgent, count) = _pending[_parsed];
            _rows.Add(new LogRow(userAgent, count, UAParser.GetClientInfo(userAgent)));
            _parsed++;

            if ((_parsed & 15) == 0 && Stopwatch.GetElapsedTime(start) >= budget)
                break;
        }

        _parseTime += Stopwatch.GetElapsedTime(start);
        return _parsed == _pending.Length;
    }

    public LogReport Report()
    {
        var rows = (_rows ?? []).OrderByDescending(r => r.Count).ToList();

        List<Slice> By(Func<ClientInfo, string?> key) =>
        [
            .. rows.GroupBy(r => key(r.Info))
                .Select(g => new Slice(g.Key, g.Sum(r => r.Count)))
                .OrderByDescending(s => s.Count)
                .ThenBy(s => s.Label is null),
        ];

        return new LogReport(
            Format ?? "unknown",
            Lines,
            Requests,
            Overflow,
            rows.Count,
            rows.Where(r => r.Info.Browser.Name is not null).Sum(r => r.Count),
            rows.Where(r => r.Info.Bot.IsBot).Sum(r => r.Count),
            _parseTime,
            Browsers: By(i => i.Browser.Name),
            BrowserVersions: By(i => Words.Browser(i.Browser)),
            Systems: By(i => i.OS.Name),
            SystemVersions: By(i => Words.OS(i.OS)),
            DeviceTypes: By(i => i.Device.Type),
            Vendors: By(i => i.Device.Vendor),
            Models: By(i => Words.DeviceName(i.Device)),
            Engines: By(i => i.Engine.Name),
            Cpus: By(i => i.CPU.Architecture),
            Bots: [.. By(i => i.Bot.Name).Where(s => s.Label is not null)],
            BotKinds: [.. By(i => i.Bot.IsBot ? Words.BotKind(i.Bot.Category) : null).Where(s => s.Label is not null)],
            Unrecognized: [.. rows.Where(r => r.Info.Browser.Name is null && !r.Info.Bot.IsBot).Take(12)],
            Rows: rows);
    }
}

/// <summary>A distinct user agent of the log, how many requests sent it, and what it is.</summary>
public sealed record LogRow(string UserAgent, long Count, ClientInfo Info);

/// <param name="Recognized">Requests whose browser was recognised.</param>
/// <param name="BotRequests">Requests from bots.</param>
/// <param name="ParseTime">Time spent parsing the distinct user agents.</param>
public sealed record LogReport(
    string Format,
    long Lines,
    long Requests,
    long Overflow,
    int Distinct,
    long Recognized,
    long BotRequests,
    TimeSpan ParseTime,
    IReadOnlyList<Slice> Browsers,
    IReadOnlyList<Slice> BrowserVersions,
    IReadOnlyList<Slice> Systems,
    IReadOnlyList<Slice> SystemVersions,
    IReadOnlyList<Slice> DeviceTypes,
    IReadOnlyList<Slice> Vendors,
    IReadOnlyList<Slice> Models,
    IReadOnlyList<Slice> Engines,
    IReadOnlyList<Slice> Cpus,
    IReadOnlyList<Slice> Bots,
    IReadOnlyList<Slice> BotKinds,
    IReadOnlyList<LogRow> Unrecognized,
    IReadOnlyList<LogRow> Rows);
