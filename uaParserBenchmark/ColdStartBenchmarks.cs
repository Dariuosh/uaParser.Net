using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

using uaParserLibrary;

namespace uaParserBenchmark;

// The very first parse in a fresh process (the parser's start-up cost), measured in 10 new
// processes.
[SimpleJob(RunStrategy.ColdStart, launchCount: 10, warmupCount: 0, iterationCount: 1)]
public class ColdStartBenchmarks
{
    private const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";

    [Benchmark]
    public object FirstClientInfo() => UAParser.GetClientInfo(UserAgent);
}
