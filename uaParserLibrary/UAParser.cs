using uaParserLibrary.Models;
using uaParserLibrary.Parsing;
using uaParserLibrary.Rules;

namespace uaParserLibrary;

/// <summary>
/// Reads the browser, engine, operating system, CPU and device from a user agent string, with
/// the rules of ua-parser-js 1.0.41. Every call returns new immutable results and keeps no state,
/// so the methods are safe to call from any number of threads.
/// </summary>
public static class UAParser
{
    /// <summary>The version of the rules, raised whenever a rule changes.</summary>
    public const string RulesVersion = UserAgentRules.RulesVersion;

    /// <summary>What the rules are based on: "ua-parser-js 1.0.41" (MIT), extended by uaParser.Net.</summary>
    public const string RulesBasedOn = UserAgentRules.BasedOn;

    /// <summary>Reads the browser from a user agent string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static Browser GetBrowser(string? userAgent) => ReadBrowser(Prepare(userAgent));

    /// <summary>Reads the processor architecture from a user agent string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static CPU GetCPU(string? userAgent) => ReadCpu(Prepare(userAgent));

    /// <summary>Reads the device from a user agent string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static Device GetDevice(string? userAgent) => ReadDevice(Prepare(userAgent));

    /// <summary>Reads the browser engine from a user agent string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static Engine GetEngine(string? userAgent) => ReadEngine(Prepare(userAgent));

    /// <summary>Reads the operating system from a user agent string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static OS GetOS(string? userAgent) => ReadOs(Prepare(userAgent));

    /// <summary>Reads the GPU from a WebGL renderer string (see <see cref="Models.GPU"/>).</summary>
    /// <param name="renderer">The WebGL renderer string; <see langword="null"/> is treated as empty.</param>
    public static GPU GetGPU(string? renderer)
    {
        var values = Rule.Apply(GpuRules.All, new Input(renderer ?? string.Empty, []));
        return new GPU(values[Field.Vendor], values[Field.Model]);
    }

    /// <summary>Reads everything from a user agent string. To reuse results for repeated user agents, use a <see cref="ClientInfoCache"/>.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    public static ClientInfo GetClientInfo(string? userAgent) => Parse(userAgent, gpu: null);

    /// <summary>Reads everything from a user agent string, and the GPU from a WebGL renderer string.</summary>
    /// <param name="userAgent">The User-Agent string; <see langword="null"/> is treated as empty.</param>
    /// <param name="renderer">A WebGL renderer string, used for <see cref="ClientInfo.GPU"/>.</param>
    public static ClientInfo GetClientInfo(string? userAgent, string? renderer) => Parse(userAgent, GetGPU(renderer));

    private static ClientInfo Parse(string? userAgent, GPU? gpu)
    {
        var input = Prepare(userAgent);
        return new ClientInfo(input.Text, ReadBrowser(input), ReadCpu(input), ReadDevice(input), ReadEngine(input), ReadOs(input), gpu);
    }

    // One Input per parse: the five rule sets share its prefilter results.
    private static Input Prepare(string? userAgent) =>
        new(JsString.NormalizeUserAgent(userAgent), UserAgentRules.PrefilterWords);

    private static Browser ReadBrowser(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Browser, ua);
        var version = values[Field.Version];
        return new Browser(values[Field.Name], version, JsString.Majorize(version));
    }

    private static CPU ReadCpu(Input ua) =>
        new(Rule.Apply(UserAgentRules.Cpu, ua)[Field.Architecture]);

    private static Device ReadDevice(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Device, ua);
        return new Device(values[Field.Vendor], values[Field.Model], values[Field.Type]);
    }

    private static Engine ReadEngine(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Engine, ua);
        return new Engine(values[Field.Name], values[Field.Version]);
    }

    private static OS ReadOs(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Os, ua);
        return new OS(values[Field.Name], values[Field.Version]);
    }
}
