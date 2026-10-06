using uaParserLibrary;

using Xunit;

using static uaParserTest.TestData;

namespace uaParserTest;

// Every user agent in the answer key must give exactly what ua-parser-js 1.0.41 gives.
// The answer key is uaParserTest/TestData/ua-parser-js.golden.json (tools/RuleGenerator).
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
