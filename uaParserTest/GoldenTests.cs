using uaParserLibrary;

using Xunit;

using static uaParserTest.TestData;

namespace uaParserTest;

// Every corpus user agent must give exactly what the rules give when tools/RuleGenerator runs
// them in JavaScript: uaParserTest/TestData/expected-results.json. This checks that the rules
// were turned into .NET regexes and assignments without changing a single result.
public class GoldenTests
{
    public static TheoryData<string> UserAgents => new(Golden.Keys);

    [Theory]
    [MemberData(nameof(UserAgents))]
    public void Matches_ua_parser_js(string userAgent)
    {
        var expected = Golden[userAgent];
        var actual = UAParser.GetClientInfo(userAgent);

        var browser = expected.GetProperty("browser");
        Assert.Equal(Value(browser, "name"), actual.Browser.Name);
        Assert.Equal(Value(browser, "version"), actual.Browser.Version);
        Assert.Equal(Value(browser, "major"), actual.Browser.Major);

        Assert.Equal(Value(expected.GetProperty("cpu"), "architecture"), actual.CPU.Architecture);

        var device = expected.GetProperty("device");
        Assert.Equal(Value(device, "vendor"), actual.Device.Vendor);
        Assert.Equal(Value(device, "model"), actual.Device.Model);
        Assert.Equal(Value(device, "type"), actual.Device.Type);

        var engine = expected.GetProperty("engine");
        Assert.Equal(Value(engine, "name"), actual.Engine.Name);
        Assert.Equal(Value(engine, "version"), actual.Engine.Version);

        var os = expected.GetProperty("os");
        Assert.Equal(Value(os, "name"), actual.OS.Name);
        Assert.Equal(Value(os, "version"), actual.OS.Version);
    }
}
