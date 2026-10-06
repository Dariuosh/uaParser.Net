using uaParserLibrary;
using uaParserLibrary.Parsing;

using Xunit;

using static uaParserTest.TestData;

namespace uaParserTest;

// Explainer (used by the demos) must describe exactly what UAParser returns.
public class ExplainerTests
{
    private const string SamsungPhone =
        "Mozilla/5.0 (Linux; Android 15; SM-S931B) AppleWebKit/537.36 (KHTML, like Gecko) " +
        "SamsungBrowser/28.0 Chrome/130.0.0.0 Mobile Safari/537.36";

    [Fact]
    public void Agrees_with_the_parser_on_every_answer_key_user_agent()
    {
        var problems = new List<string>();

        foreach (var userAgent in Golden.Keys)
        {
            var info = UAParser.GetClientInfo(userAgent);
            var explanation = Explainer.Explain(userAgent);

            Assert.Equal(info.UserAgent, explanation.UserAgent);

            Check("Browser", ("Name", info.Browser.Name), ("Version", info.Browser.Version));
            Check("Engine", ("Name", info.Engine.Name), ("Version", info.Engine.Version));
            Check("OS", ("Name", info.OS.Name), ("Version", info.OS.Version));
            Check("Device", ("Vendor", info.Device.Vendor), ("Model", info.Device.Model), ("Type", info.Device.Type));
            Check("CPU", ("Architecture", info.CPU.Architecture));

            void Check(string part, params (string Field, string? Value)[] expected)
            {
                var hit = explanation.Parts.Single(p => p.Part == part).Hit;
                var values = hit?.Values.ToDictionary(v => v.Field, v => v.Value) ?? [];

                foreach (var (field, value) in expected)
                {
                    var explained = values.GetValueOrDefault(field);
                    if (explained != value)
                        problems.Add($"{part}.{field}: parser '{value}', explainer '{explained}' for {userAgent}");
                }

                if (hit is null)
                    return;

                // Every value read from the text lies inside the match.
                foreach (var source in hit.Values.Where(v => v.Index >= 0))
                {
                    if (source.Index < hit.Index || source.Index + source.Length > hit.Index + hit.Length)
                        problems.Add($"{part}.{source.Field}: span outside the match for {userAgent}");
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems.Take(20)));
    }

    [Fact]
    public void Shows_where_each_value_comes_from()
    {
        var explanation = Explainer.Explain(SamsungPhone);
        var ua = explanation.UserAgent;

        var browser = explanation.Parts.Single(p => p.Part == "Browser").Hit!;
        Assert.Contains("samsungbrowser", browser.Pattern);
        Assert.EndsWith("/i", browser.Pattern);
        Assert.Equal("SamsungBrowser/28.0", ua.Substring(browser.Index, browser.Length));

        var version = browser.Values.Single(v => v.Field == "Version");
        Assert.Equal("28.0", ua.Substring(version.Index, version.Length));
        Assert.Equal("captured", version.How);

        var device = explanation.Parts.Single(p => p.Part == "Device").Hit!;
        var model = device.Values.Single(v => v.Field == "Model");
        Assert.Equal("SM-S931B", ua.Substring(model.Index, model.Length));

        // The vendor is not in the text: the rule sets it.
        var vendor = device.Values.Single(v => v.Field == "Vendor");
        Assert.Equal("Samsung", vendor.Value);
        Assert.Equal(-1, vendor.Index);
        Assert.Equal("set by the rule", vendor.How);

        Assert.Null(explanation.Parts.Single(p => p.Part == "CPU").Hit);
    }

    [Fact]
    public void Counts_rules_from_one()
    {
        var explanation = Explainer.Explain(SamsungPhone);

        foreach (var part in explanation.Parts)
        {
            Assert.True(part.RuleCount > 0);
            if (part.Hit is { } hit)
            {
                Assert.InRange(hit.RuleNumber, 1, part.RuleCount);
                Assert.InRange(hit.RegexNumber, 1, hit.RegexCount);
            }
        }
    }
}
