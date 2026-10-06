using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Running;

using uaParserBenchmark;

// dotnet run -c Release -- --quick      a quick table (about half a minute)
// dotnet run -c Release                 BenchmarkDotNet (choose the benchmarks to run)
// dotnet run -c Release -- --filter *   all BenchmarkDotNet benchmarks
// Results are saved in benchmark-results/ at the repository root (--out <folder> to change).
if (args.Contains("--quick"))
{
    QuickRun.Run(args);
}
else
{
    var folder = Results.Folder(args);
    var rest = RemoveOut(args);
    var config = DefaultConfig.Instance
        .AddExporter(JsonExporter.Full)
        .WithArtifactsPath(Path.Combine(folder, "BenchmarkDotNet"));
    BenchmarkSwitcher.FromAssembly(typeof(Corpus).Assembly).Run(rest, config);
}

static string[] RemoveOut(string[] args)
{
    var index = Array.IndexOf(args, "--out");
    return index < 0 ? args : [.. args[..index], .. args[Math.Min(index + 2, args.Length)..]];
}
