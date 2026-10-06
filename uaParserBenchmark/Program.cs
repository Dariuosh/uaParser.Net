using BenchmarkDotNet.Running;

using uaParserBenchmark;

// dotnet run -c Release -- --quick    a quick table (about half a minute)
// dotnet run -c Release               BenchmarkDotNet (choose the benchmarks to run)
// dotnet run -c Release -- --filter * all BenchmarkDotNet benchmarks
if (args.Contains("--quick"))
{
    QuickRun.Run();
}
else
{
    BenchmarkSwitcher.FromAssembly(typeof(Corpus).Assembly).Run(args);
}
