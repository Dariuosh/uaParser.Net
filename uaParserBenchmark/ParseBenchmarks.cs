using BenchmarkDotNet.Attributes;

using uaParserLibrary;

namespace uaParserBenchmark;

// Steady-state cost of parsing one user agent. Each operation parses the same 500 user agents
// (a corpus repeated if needed), and the results are reported per user agent.
[MemoryDiagnoser]
public class ParseBenchmarks
{
    private const int UserAgentsPerOperation = 500;

    private string[] _userAgents = [];
    private ClientInfoCache _cache = new();

    [ParamsSource(nameof(Corpora))]
    public string Corpus { get; set; } = uaParserBenchmark.Corpus.Legacy;

    public static IEnumerable<string> Corpora => uaParserBenchmark.Corpus.Names;

    [GlobalSetup]
    public void Setup()
    {
        _userAgents = uaParserBenchmark.Corpus.Sample(Corpus, UserAgentsPerOperation);
        _cache = new ClientInfoCache(capacity: 4096);
        foreach (var ua in _userAgents)
            _cache.GetClientInfo(ua);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = UserAgentsPerOperation)]
    public object? ClientInfo()
    {
        object? last = null;
        foreach (var ua in _userAgents)
            last = UAParser.GetClientInfo(ua);
        return last;
    }

    // Repeated user agents, as on a busy server: every one is already in the cache.
    [Benchmark(OperationsPerInvoke = UserAgentsPerOperation)]
    public object? CachedClientInfo()
    {
        object? last = null;
        foreach (var ua in _userAgents)
            last = _cache.GetClientInfo(ua);
        return last;
    }

    [Benchmark(OperationsPerInvoke = UserAgentsPerOperation)]
    public object? Browser()
    {
        object? last = null;
        foreach (var ua in _userAgents)
            last = UAParser.GetBrowser(ua);
        return last;
    }

    [Benchmark(OperationsPerInvoke = UserAgentsPerOperation)]
    public object? OS()
    {
        object? last = null;
        foreach (var ua in _userAgents)
            last = UAParser.GetOS(ua);
        return last;
    }

    [Benchmark(OperationsPerInvoke = UserAgentsPerOperation)]
    public object? Device()
    {
        object? last = null;
        foreach (var ua in _userAgents)
            last = UAParser.GetDevice(ua);
        return last;
    }
}
