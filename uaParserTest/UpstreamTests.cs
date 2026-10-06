using uaParserLibrary;

using Xunit;

using static uaParserTest.TestData;

namespace uaParserTest;

// ua-parser-js 1.0.41's own test cases, checked the way its test.js checks them:
// an expected "undefined" or a missing key means no value (null).
public class UpstreamTests
{
    public static TheoryData<string, int, string> Cases()
    {
        var data = new TheoryData<string, int, string>();
        foreach (var category in new[] { "browser", "cpu", "device", "engine", "os" })
        {
            var cases = Upstream(category);
            for (var i = 0; i < cases.Length; i++)
                data.Add(category, i, cases[i].GetProperty("desc").GetString()!);
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Matches_upstream_expectation(string category, int index, string description)
    {
        _ = description; // shown in the test name only
        var testCase = Upstream(category)[index];
        var ua = testCase.GetProperty("ua").GetString();
        var expect = testCase.GetProperty("expect");

        switch (category)
        {
            case "browser":
                var browser = UAParser.GetBrowser(ua);
                Assert.Equal(Value(expect, "name"), browser.Name);
                Assert.Equal(Value(expect, "version"), browser.Version);
                Assert.Equal(Value(expect, "major"), browser.Major);
                break;
            case "cpu":
                Assert.Equal(Value(expect, "architecture"), UAParser.GetCPU(ua).Architecture);
                break;
            case "device":
                var device = UAParser.GetDevice(ua);
                Assert.Equal(Value(expect, "vendor"), device.Vendor);
                Assert.Equal(Value(expect, "model"), device.Model);
                Assert.Equal(Value(expect, "type"), device.Type);
                break;
            case "engine":
                var engine = UAParser.GetEngine(ua);
                Assert.Equal(Value(expect, "name"), engine.Name);
                Assert.Equal(Value(expect, "version"), engine.Version);
                break;
            case "os":
                var os = UAParser.GetOS(ua);
                Assert.Equal(Value(expect, "name"), os.Name);
                Assert.Equal(Value(expect, "version"), os.Version);
                break;
            default:
                Assert.Fail($"Unknown category {category}");
                break;
        }
    }
}
