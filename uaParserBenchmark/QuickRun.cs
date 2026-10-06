using System.Diagnostics;
using System.Text;

using uaParserLibrary;

namespace uaParserBenchmark;

// A quick measurement (about half a minute) without BenchmarkDotNet. Same method as
// tools/RuleGenerator's `npm run bench`, so the numbers can be compared with ua-parser-js.
// Prints the results and saves them as Markdown and JSON.
public static class QuickRun
{
    public static void Run(string[] args)
    {
        // Measured first, before anything else has used the parser.
        var cold = Stopwatch.StartNew();
        UAParser.GetClientInfo(Corpus.Load(Corpus.Legacy)[0]);
        cold.Stop();

        var machine = Results.Machine();
        var rows = Corpus.Names.Select(name =>
        {
            var userAgents = Corpus.Load(name);
            var (min, median) = Measure(userAgents);
            return new Row(name, userAgents.Length, Math.Round(min, 1), Math.Round(median, 1));
        }).ToList();

        var report = new StringBuilder()
            .AppendLine($"# uaParser.Net quick benchmark")
            .AppendLine()
            .AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm zzz}")
            .AppendLine($"- Library: uaParser.Net, rules from ua-parser-js {UAParser.RulesVersion}")
            .AppendLine($"- Runtime: {machine["runtime"]}")
            .AppendLine($"- Machine: {machine["cpu"]}, {machine["cpus"]} CPUs, {machine["os"]} ({machine["architecture"]})")
            .AppendLine()
            .AppendLine($"First call (cold start): **{cold.Elapsed.TotalMilliseconds:F0} ms**")
            .AppendLine()
            .AppendLine("Per user agent, after warm-up (15 trials):")
            .AppendLine()
            .AppendLine("| Corpus | User agents | min (µs) | median (µs) |")
            .AppendLine("|---|---:|---:|---:|");
        foreach (var row in rows)
            report.AppendLine($"| {row.Corpus} | {row.UserAgents} | {row.MinMicroseconds:F1} | {row.MedianMicroseconds:F1} |");

        Console.Write(report);

        var folder = Results.Folder(args);
        Directory.CreateDirectory(folder);
        var stem = Path.Combine(folder, $"uaParser.Net-quick-{DateTime.Now:yyyyMMdd-HHmmss}");
        File.WriteAllText(stem + ".md", report.ToString());
        Results.WriteJson(stem + ".json", new
        {
            library = "uaParser.Net",
            rules = $"ua-parser-js {UAParser.RulesVersion}",
            date = DateTimeOffset.Now,
            machine,
            coldStartMilliseconds = Math.Round(cold.Elapsed.TotalMilliseconds, 1),
            perUserAgent = rows,
        });
        Console.WriteLine();
        Console.WriteLine($"Saved: {stem}.md and .json");
    }

    private sealed record Row(string Corpus, int UserAgents, double MinMicroseconds, double MedianMicroseconds);

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
