using System.Globalization;

using uaParserLibrary;
using uaParserLibrary.Models;

using Xunit;

namespace uaParserTest;

public class ParserTests
{
    private const string Chrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
    private const string Firefox =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:145.0) Gecko/20100101 Firefox/145.0";
    private const string IPhone =
        "Mozilla/5.0 (iPhone; CPU iPhone OS 18_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.6 Mobile/15E148 Safari/604.1";

    [Fact]
    public void Reads_every_part_of_a_user_agent()
    {
        var info = UAParser.GetClientInfo(IPhone);

        Assert.Equal(IPhone, info.UserAgent);
        Assert.Equal(new Browser("Mobile Safari", "18.6", "18"), info.Browser);
        Assert.Equal(new Device("Apple", "iPhone", DeviceTypes.Mobile), info.Device);
        Assert.Equal(new Engine("WebKit", "605.1.15"), info.Engine);
        Assert.Equal(new OS("iOS", "18.6"), info.OS);
        Assert.Equal(new CPU(null), info.CPU);
        Assert.Null(info.GPU);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a user agent")]
    public void Unknown_input_gives_null_values(string? userAgent)
    {
        var info = UAParser.GetClientInfo(userAgent);

        Assert.Equal(userAgent ?? "", info.UserAgent);
        Assert.Equal(new Browser(null, null, null), info.Browser);
        Assert.Equal(new CPU(null), info.CPU);
        Assert.Equal(new Device(null, null, null), info.Device);
        Assert.Equal(new Engine(null, null), info.Engine);
        Assert.Equal(new OS(null, null), info.OS);
    }

    [Fact]
    public void Long_user_agents_are_cut_to_500_characters_like_ua_parser_js()
    {
        var info = UAParser.GetClientInfo("   " + Chrome + new string('x', 1000));

        Assert.Equal(500, info.UserAgent.Length);
        Assert.StartsWith("Mozilla/5.0", info.UserAgent);
        Assert.Equal("Chrome", info.Browser.Name);
    }

    [Fact]
    public void Short_user_agents_keep_their_leading_whitespace()
    {
        Assert.Equal("  " + Chrome, UAParser.GetClientInfo("  " + Chrome).UserAgent);
    }

    [Fact]
    public void A_huge_hostile_user_agent_is_parsed_quickly()
    {
        var hostile = "Mozilla/5.0 (" + new string(' ', 100_000) + "x";

        var elapsed = System.Diagnostics.Stopwatch.StartNew();
        UAParser.GetClientInfo(hostile);

        Assert.True(elapsed.ElapsedMilliseconds < 1000, $"took {elapsed.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void Every_call_returns_its_own_result()
    {
        var first = UAParser.GetBrowser(Chrome);
        var second = UAParser.GetBrowser(Firefox);

        Assert.Equal("Chrome", first.Name);
        Assert.Equal("Firefox", second.Name);
        Assert.NotSame(UAParser.GetBrowser(Chrome), UAParser.GetBrowser(Chrome));
    }

    [Fact]
    public void Concurrent_calls_do_not_interfere()
    {
        var wrong = 0;

        Parallel.For(0, 20_000, i =>
        {
            var (ua, name) = (i % 3) switch
            {
                0 => (Chrome, "Chrome"),
                1 => (Firefox, "Firefox"),
                _ => (IPhone, "Mobile Safari"),
            };
            if (UAParser.GetClientInfo(ua).Browser.Name != name)
                Interlocked.Increment(ref wrong);
        });

        Assert.Equal(0, wrong);
    }

    [Fact]
    public void Matching_does_not_depend_on_the_current_culture()
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            // In Turkish, "I" lowercases to a dotless "ı", which breaks naive case-insensitive matching.
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            var ie = UAParser.GetClientInfo("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)");

            Assert.Equal("IE", ie.Browser.Name);
            Assert.Equal("Windows", ie.OS.Name);
            Assert.Equal("7", ie.OS.Version);
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }

    [Fact]
    public void Device_types_cover_every_type_in_the_answer_key()
    {
        var known = typeof(DeviceTypes).GetFields().Select(f => (string)f.GetValue(null)!).ToHashSet();
        var seen = TestData.Golden.Values
            .Select(c => TestData.Value(c.GetProperty("device"), "type"))
            .OfType<string>()
            .ToHashSet();

        Assert.Subset(known, seen);
    }

    [Fact]
    public void Results_read_well_as_text()
    {
        var info = UAParser.GetClientInfo(Chrome);

        Assert.Equal("Browser: Chrome 140.0.0.0", info.Browser.ToString());
        Assert.Equal("OS     : Windows 10", info.OS.ToString());
        Assert.Equal("Device : ", info.Device.ToString());
    }

    [Fact]
    public void Rules_are_based_on_ua_parser_js_1_0_41()
    {
        Assert.Equal("ua-parser-js 1.0.41", UAParser.RulesBasedOn);
        Assert.Matches(@"^\d+\.\d+\.\d+$", UAParser.RulesVersion);
    }
}
