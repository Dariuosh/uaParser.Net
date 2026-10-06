using uaParserLibrary;

using Xunit;

using static uaParserTest.TestData;

namespace uaParserTest;

// ua-parser-js 1.0.41's own test cases, checked the way its test.js checks them:
// an expected "undefined" or a missing key means no value (null). Where uaParser.Net changes a
// result on purpose, tools/RuleGenerator/corpus/upstream-test-differences.json gives the new value.
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
        var testCase = Upstream(category)[index];
        var ua = testCase.GetProperty("ua").GetString();
        var expect = testCase.GetProperty("expect");

        string? Expected(string field) =>
            UpstreamDifferences.TryGetValue((category, description, field), out var ours) ? ours : Value(expect, field);

        switch (category)
        {
            case "browser":
                var browser = UAParser.GetBrowser(ua);
                Assert.Equal(Expected("name"), browser.Name);
                Assert.Equal(Expected("version"), browser.Version);
                Assert.Equal(Expected("major"), browser.Major);
                break;
            case "cpu":
                Assert.Equal(Expected("architecture"), UAParser.GetCPU(ua).Architecture);
                break;
            case "device":
                var device = UAParser.GetDevice(ua);
                Assert.Equal(Expected("vendor"), device.Vendor);
                Assert.Equal(Expected("model"), device.Model);
                Assert.Equal(Expected("type"), device.Type);
                break;
            case "engine":
                var engine = UAParser.GetEngine(ua);
                Assert.Equal(Expected("name"), engine.Name);
                Assert.Equal(Expected("version"), engine.Version);
                break;
            case "os":
                var os = UAParser.GetOS(ua);
                Assert.Equal(Expected("name"), os.Name);
                Assert.Equal(Expected("version"), os.Version);
                break;
            default:
                Assert.Fail($"Unknown category {category}");
                break;
        }
    }
}
