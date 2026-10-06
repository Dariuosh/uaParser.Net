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
    /// <summary>The ua-parser-js version the rules come from.</summary>
    public const string RulesVersion = UserAgentRules.UpstreamVersion;

    public static Browser GetBrowser(string? userAgent) => ReadBrowser(Prepare(userAgent));

    public static CPU GetCPU(string? userAgent) => ReadCpu(Prepare(userAgent));

    public static Device GetDevice(string? userAgent) => ReadDevice(Prepare(userAgent));

    public static Engine GetEngine(string? userAgent) => ReadEngine(Prepare(userAgent));

    public static OS GetOS(string? userAgent) => ReadOs(Prepare(userAgent));

    /// <summary>Reads the GPU from a WebGL renderer string (see <see cref="Models.GPU"/>).</summary>
    public static GPU GetGPU(string? renderer)
    {
        var values = Rule.Apply(GpuRules.All, new Input(renderer ?? string.Empty, []));
        return new GPU(values[(int)Field.Vendor], values[(int)Field.Model]);
    }

    public static ClientInfo GetClientInfo(string? userAgent) => Parse(userAgent, gpu: null);

    /// <param name="userAgent">The user agent string.</param>
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
        var version = values[(int)Field.Version];
        return new Browser(values[(int)Field.Name], version, JsString.Majorize(version));
    }

    private static CPU ReadCpu(Input ua) =>
        new(Rule.Apply(UserAgentRules.Cpu, ua)[(int)Field.Architecture]);

    private static Device ReadDevice(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Device, ua);
        return new Device(values[(int)Field.Vendor], values[(int)Field.Model], values[(int)Field.Type]);
    }

    private static Engine ReadEngine(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Engine, ua);
        return new Engine(values[(int)Field.Name], values[(int)Field.Version]);
    }

    private static OS ReadOs(Input ua)
    {
        var values = Rule.Apply(UserAgentRules.Os, ua);
        return new OS(values[(int)Field.Name], values[(int)Field.Version]);
    }
}
