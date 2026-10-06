using System.Diagnostics;

using uaParserLibrary;

namespace uaParserBenchmark;

// A quick measurement (about half a minute) without BenchmarkDotNet. Same method as
// tools/RuleGenerator's `npm run bench`, so the numbers can be compared with ua-parser-js.
public static class QuickRun
{
    public static void Run()
    {
        // Measured first, before anything else has used the parser.
        var cold = Stopwatch.StartNew();
        UAParser.GetClientInfo(Corpus.Load(Corpus.Legacy)[0]);
        cold.Stop();

        Console.WriteLine($"uaParser.Net quick benchmark: rules from ua-parser-js {UAParser.RulesVersion}, {Environment.Version}, {Environment.ProcessorCount} CPUs");
        Console.WriteLine($"First call (cold start): {cold.Elapsed.TotalMilliseconds:F0} ms");
        Console.WriteLine();
        Console.WriteLine("Per user agent, after warm-up (15 trials):");
        Console.WriteLine($"{"Corpus",-20} {"User agents",11} {"min",9} {"median",9}");

        foreach (var name in Corpus.Names)
        {
            var userAgents = Corpus.Load(name);
            var (min, median) = Measure(userAgents);
            Console.WriteLine($"{name,-20} {userAgents.Length,11} {min,6:F1} µs {median,6:F1} µs");
        }

        Console.WriteLine();
        Console.WriteLine("Full benchmarks (BenchmarkDotNet): dotnet run -c Release");
    }

    private static (double Min, double Median) Measure(string[] userAgents)
    {
        // Enough warm-up for tiered compilation to optimise the parser's hot paths.
        for (var r = 0; r < 40; r++)
            Parse(userAgents);
        Thread.Sleep(500);
        for (var r = 0; r < 10; r++)
            Parse(userAgents);

        var trials = new double[15];
        for (var t = 0; t < trials.Length; t++)
        {
            var watch = Stopwatch.StartNew();
            for (var r = 0; r < 3; r++)
                Parse(userAgents);
            trials[t] = watch.Elapsed.TotalMicroseconds / (3.0 * userAgents.Length);
        }

        Array.Sort(trials);
        return (trials[0], trials[trials.Length / 2]);
    }

    private static void Parse(string[] userAgents)
    {
        foreach (var ua in userAgents)
            UAParser.GetClientInfo(ua);
    }
}
