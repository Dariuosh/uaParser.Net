using System.Runtime.InteropServices;
using System.Text.Json;

namespace uaParserBenchmark;

// Where results go and what machine produced them.
public static class Results
{
    // benchmark-results/ at the repository root (found from the current directory), or under
    // the current directory when not inside the repository. `--out <folder>` overrides it.
    public static string Folder(string[] args)
    {
        var index = Array.IndexOf(args, "--out");
        if (index >= 0 && index + 1 < args.Length)
            return Path.GetFullPath(args[index + 1]);

        for (var dir = new DirectoryInfo(Environment.CurrentDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "uaParser.slnx")))
                return Path.Combine(dir.FullName, "benchmark-results");
        }
        return Path.Combine(Environment.CurrentDirectory, "benchmark-results");
    }

    public static Dictionary<string, string> Machine() => new()
    {
        ["cpu"] = CpuName(),
        ["cpus"] = Environment.ProcessorCount.ToString(),
        ["os"] = RuntimeInformation.OSDescription,
        ["architecture"] = RuntimeInformation.ProcessArchitecture.ToString(),
        ["runtime"] = RuntimeInformation.FrameworkDescription,
    };

    public static void WriteJson(string file, object value) =>
        File.WriteAllText(file, JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) + Environment.NewLine);

    private static string CpuName()
    {
        try
        {
            if (OperatingSystem.IsLinux() && File.Exists("/proc/cpuinfo"))
            {
                var line = File.ReadLines("/proc/cpuinfo").FirstOrDefault(l => l.StartsWith("model name", StringComparison.Ordinal));
                if (line is not null)
                    return line[(line.IndexOf(':') + 1)..].Trim();
            }
            if (Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") is { Length: > 0 } windows)
                return windows;
        }
        catch (IOException)
        {
        }
        return RuntimeInformation.ProcessArchitecture.ToString();
    }
}
