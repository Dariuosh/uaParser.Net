using uaParserDemoComponents.AccessLogs;
using uaParserDemoComponents.Playground;
using uaParserDemoComponents.Results;

using uaParserSamples;

using Xunit;

namespace uaParserTest;

// The parts of the demos that are logic rather than layout.
public class DemoTests
{
    private const string Chrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";

    [Theory]
    // Apache and nginx "combined"
    [InlineData($"""203.0.113.7 - - [01/Oct/2026:10:00:00 +0000] "GET / HTTP/1.1" 200 512 "-" "{Chrome}" """)]
    // nginx "main": X-Forwarded-For after the user agent
    [InlineData($"""203.0.113.7 - - [01/Oct/2026:10:00:00 +0000] "GET / HTTP/1.1" 200 512 "https://example.com/" "{Chrome}" "198.51.100.1" """)]
    // JSON lines, Caddy style
    [InlineData($$$"""{"level":"info","request":{"headers":{"User-Agent":["{{{Chrome}}}"]}},"status":200}""")]
    // JSON lines, flat
    [InlineData($$"""{"time":"2026-10-01T10:00:00Z","http_user_agent":"{{Chrome}}"}""")]
    // One user agent per line, plain or quoted
    [InlineData(Chrome)]
    [InlineData($"\"{Chrome}\"")]
    public void Finds_the_user_agent_in_a_log_line(string line)
    {
        Assert.Equal(Chrome, new LogLineReader().Read(line));
    }

    [Fact]
    public void Reads_IIS_logs_by_their_Fields_line()
    {
        var reader = new LogLineReader();
        Assert.Null(reader.Read("#Software: Microsoft Internet Information Services 10.0"));
        Assert.Null(reader.Read("#Fields: date time s-ip cs-method cs-uri-stem cs(User-Agent) sc-status"));

        var line = $"2026-10-01 10:00:00 192.0.2.1 GET / {Chrome.Replace(' ', '+')} 200";
        Assert.Equal(Chrome, reader.Read(line));
        Assert.Equal("IIS (W3C)", reader.Format);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("203.0.113.7 - - [01/Oct/2026:10:00:00 +0000] \"GET / HTTP/1.1\" 200 512 \"-\" \"-\"")]
    [InlineData("203.0.113.7 - - [01/Oct/2026:10:00:00 +0000] \"GET / HTTP/1.1\" 200 512")]
    public void Lines_without_a_user_agent_give_null(string line)
    {
        Assert.Null(new LogLineReader().Read(line));
    }

    [Fact]
    public void Unescapes_quotes_in_combined_logs()
    {
        var line = """203.0.113.7 - - [01/Oct/2026:10:00:00 +0000] "GET / HTTP/1.1" 200 1 "-" "a \"quoted\" agent \x22x\x22" """;
        Assert.Equal("a \"quoted\" agent \"x\"", new LogLineReader().Read(line));
    }

    [Fact]
    public void Analyzes_the_example_log()
    {
        var analysis = new LogAnalysis();
        foreach (var line in ExampleLog.Lines())
            analysis.AddLine(line);

        while (!analysis.ParseSome(TimeSpan.FromMilliseconds(5)))
        {
        }

        var report = analysis.Report();
        Assert.Equal(ExampleLog.DefaultLines, report.Lines);
        Assert.Equal(ExampleLog.DefaultLines, report.Requests);
        Assert.Equal("Apache / nginx (combined)", report.Format);
        Assert.Equal(report.Requests, report.Browsers.Sum(s => s.Count));
        Assert.Equal(report.Distinct, report.Rows.Count);
        Assert.All(report.Rows, row => Assert.Contains(SampleUserAgents.All, s => s.UserAgent == row.UserAgent));
    }

    // The editor's colored copy must contain exactly the typed text, or the colors would drift.
    [Fact]
    public void Highlighting_keeps_the_text_intact()
    {
        string[] texts =
        [
            .. SampleUserAgents.All.Select(s => s.UserAgent),
            "  " + Chrome + "  ",
            Chrome + "\n",
            new string('x', 600) + " " + Chrome,
            "",
        ];

        foreach (var text in texts)
        {
            var segments = Highlight.Split(text, ParseTrace.Of(text));
            Assert.Equal(text, string.Concat(segments.Select(s => s.Text)));
        }
    }

    [Fact]
    public void Highlights_the_captured_values()
    {
        const string ua = "Mozilla/5.0 (Linux; Android 15; SM-S931B) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/28.0 Chrome/130.0.0.0 Mobile Safari/537.36";
        var segments = Highlight.Split("  " + ua, ParseTrace.Of(ua));

        Assert.Contains(segments, s => s is { Text: "SM-S931B", Part: "Device", Field: "Model" });
        Assert.Contains(segments, s => s is { Text: "28.0", Part: "Browser", Field: "Version" });
        Assert.Contains(segments, s => s is { Text: "Android", Part: "OS", Field: "Name" });
    }
}
