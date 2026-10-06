using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using uaParserLibrary;
using uaParserLibrary.Models;

using uaParserMiddleware;

using Xunit;

namespace uaParserTest;

public class ClientHintsTests
{
    private const string WindowsChrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
    private const string MacChrome =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
    private const string AndroidChrome =
        "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Mobile Safari/537.36";

    // Request headers as Chrome 140 on Windows 11 sends them after Accept-CH.
    private static readonly Dictionary<string, string> Windows11 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Sec-CH-UA"] = "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Google Chrome\";v=\"140\"",
        ["Sec-CH-UA-Mobile"] = "?0",
        ["Sec-CH-UA-Platform"] = "\"Windows\"",
        ["Sec-CH-UA-Full-Version-List"] = "\"Chromium\";v=\"140.0.7339.128\", \"Not=A?Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"140.0.7339.128\"",
        ["Sec-CH-UA-Platform-Version"] = "\"19.0.0\"",
        ["Sec-CH-UA-Arch"] = "\"x86\"",
        ["Sec-CH-UA-Bitness"] = "\"64\"",
        ["Sec-CH-UA-Model"] = "\"\"",
        ["Sec-CH-UA-Form-Factors"] = "\"Desktop\"",
        ["Sec-CH-UA-WoW64"] = "?0",
    };

    private static ClientHints Hints(Dictionary<string, string> headers) =>
        ClientHints.FromHeaders(name => headers.GetValueOrDefault(name))!;

    private static Dictionary<string, string> With(Dictionary<string, string> headers, params (string Name, string Value)[] changes)
    {
        var copy = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
        foreach (var (name, value) in changes)
            copy[name] = value;
        return copy;
    }

    [Fact]
    public void Reads_the_headers()
    {
        var hints = Hints(Windows11);

        Assert.Equal([new("Chromium", "140"), new("Not=A?Brand", "24"), new("Google Chrome", "140")], hints.Brands);
        Assert.Equal(new BrandVersion("Google Chrome", "140.0.7339.128"), hints.FullVersionList[2]);
        Assert.False(hints.Mobile);
        Assert.Equal("Windows", hints.Platform);
        Assert.Equal("19.0.0", hints.PlatformVersion);
        Assert.Equal("x86", hints.Architecture);
        Assert.Equal("64", hints.Bitness);
        Assert.Null(hints.Model);
        Assert.Equal(["Desktop"], hints.FormFactors);
        Assert.False(hints.Wow64);
        Assert.Equal("Hints  : Chromium 140.0.7339.128, Google Chrome 140.0.7339.128, Windows 19.0.0, x86 64, Desktop", hints.ToString());
    }

    [Fact]
    public void Windows_11_full_version_and_cpu()
    {
        var info = UAParser.GetClientInfo(WindowsChrome, Hints(Windows11));

        Assert.Equal(new Browser("Chrome", "140.0.7339.128", "140"), info.Browser);
        Assert.Equal(new Engine("Blink", "140.0.7339.128"), info.Engine);
        Assert.Equal(new OS("Windows", "11"), info.OS);
        Assert.Equal(new CPU("amd64"), info.CPU);
        Assert.Equal(new Device(null, null, null), info.Device);
        Assert.Equal(Hints(Windows11), info.Hints);
    }

    [Theory]
    [InlineData("10.0.0", "10")]
    [InlineData("1.0.0", "10")]
    [InlineData("13.0.0", "11")]
    [InlineData("0.3.0", "8.1")]
    [InlineData("0.1.0", "7")]
    public void Windows_versions(string platformVersion, string version)
    {
        var hints = Hints(With(Windows11, ("Sec-CH-UA-Platform-Version", $"\"{platformVersion}\"")));

        Assert.Equal(new OS("Windows", version), UAParser.GetClientInfo(WindowsChrome, hints).OS);
    }

    [Fact]
    public void Windows_on_arm()
    {
        var hints = Hints(With(Windows11, ("Sec-CH-UA-Arch", "\"arm\"")));

        Assert.Equal(new CPU("arm64"), UAParser.GetClientInfo(WindowsChrome, hints).CPU);
    }

    [Fact]
    public void Brave_on_a_mac_with_apple_silicon()
    {
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA"] = "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Brave\";v=\"140\"",
            ["Sec-CH-UA-Mobile"] = "?0",
            ["Sec-CH-UA-Platform"] = "\"macOS\"",
            ["Sec-CH-UA-Platform-Version"] = "\"15.6.1\"",
            ["Sec-CH-UA-Arch"] = "\"arm\"",
            ["Sec-CH-UA-Bitness"] = "\"64\"",
        });

        var info = UAParser.GetClientInfo(MacChrome, hints);

        Assert.Equal(new Browser("Brave", "140.0.0.0", "140"), info.Browser);
        Assert.Equal(new OS("Mac OS", "15.6.1"), info.OS);
        Assert.Equal(new CPU("arm64"), info.CPU);
        Assert.Equal(new Device("Apple", "Macintosh", null), info.Device);
    }

    [Fact]
    public void Android_phone_model()
    {
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA"] = "\"Google Chrome\";v=\"140\", \"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\"",
            ["Sec-CH-UA-Mobile"] = "?1",
            ["Sec-CH-UA-Platform"] = "\"Android\"",
            ["Sec-CH-UA-Platform-Version"] = "\"16.0.0\"",
            ["Sec-CH-UA-Model"] = "\"Pixel 9 Pro\"",
            ["Sec-CH-UA-Full-Version-List"] = "\"Google Chrome\";v=\"140.0.7339.155\", \"Chromium\";v=\"140.0.7339.155\", \"Not=A?Brand\";v=\"24.0.0.0\"",
        });

        var info = UAParser.GetClientInfo(AndroidChrome, hints);

        Assert.Equal(new Browser("Chrome", "140.0.7339.155", "140"), info.Browser);
        Assert.Equal(new OS("Android", "16"), info.OS);
        Assert.Equal(new Device("Google", "Pixel 9 Pro", DeviceTypes.Mobile), info.Device);
    }

    [Fact]
    public void Samsung_tablet_with_samsung_internet()
    {
        const string userAgent =
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/28.0 Chrome/130.0.0.0 Safari/537.36";
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA"] = "\"Chromium\";v=\"130\", \"Samsung Internet\";v=\"28.0\", \"Not?A_Brand\";v=\"99\"",
            ["Sec-CH-UA-Mobile"] = "?0",
            ["Sec-CH-UA-Platform"] = "\"Android\"",
            ["Sec-CH-UA-Platform-Version"] = "\"15.0.0\"",
            ["Sec-CH-UA-Model"] = "\"SM-X710\"",
            ["Sec-CH-UA-Full-Version-List"] = "\"Chromium\";v=\"130.0.6723.86\", \"Samsung Internet\";v=\"28.0.1.15\", \"Not?A_Brand\";v=\"99.0.0.0\"",
        });

        var info = UAParser.GetClientInfo(userAgent, hints);

        Assert.Equal(new Browser("Samsung Internet", "28.0.1.15", "28"), info.Browser);
        Assert.Equal(new Engine("Blink", "130.0.6723.86"), info.Engine);
        Assert.Equal(new OS("Android", "15"), info.OS);
        Assert.Equal(new Device("Samsung", "SM-X710", DeviceTypes.Tablet), info.Device);
    }

    [Fact]
    public void Edge_keeps_its_name()
    {
        const string userAgent = WindowsChrome + " Edg/140.0.0.0";
        var hints = Hints(With(Windows11,
            ("Sec-CH-UA", "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Microsoft Edge\";v=\"140\""),
            ("Sec-CH-UA-Full-Version-List", "\"Chromium\";v=\"140.0.7339.128\", \"Not=A?Brand\";v=\"24.0.0.0\", \"Microsoft Edge\";v=\"140.0.3485.66\"")));

        Assert.Equal(new Browser("Edge", "140.0.3485.66", "140"), UAParser.GetClientInfo(userAgent, hints).Browser);
    }

    [Fact]
    public void Low_entropy_hints_do_not_lose_the_user_agent_version()
    {
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA"] = Windows11["Sec-CH-UA"],
            ["Sec-CH-UA-Mobile"] = "?0",
            ["Sec-CH-UA-Platform"] = "\"Windows\"",
        });

        var info = UAParser.GetClientInfo(WindowsChrome, hints);

        Assert.Equal(UAParser.GetClientInfo(WindowsChrome) with { Hints = hints }, info);
    }

    [Fact]
    public void Hints_without_a_user_agent()
    {
        var info = UAParser.GetClientInfo(null, Hints(Windows11));

        Assert.Equal(new Browser("Chrome", "140.0.7339.128", "140"), info.Browser);
        Assert.Equal(new Engine("Blink", "140.0.7339.128"), info.Engine);
        Assert.Equal(new OS("Windows", "11"), info.OS);
    }

    [Fact]
    public void A_more_precise_user_agent_wins()
    {
        // HarmonyOS phones report Android as the platform.
        const string userAgent =
            "Mozilla/5.0 (Linux; Android 12; HarmonyOS; ALN-AL00; HMSCore 6.13.0.302) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.196 HuaweiBrowser/15.0.4.312 Mobile Safari/537.36";
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA"] = "\"Chromium\";v=\"114\", \"Not.A/Brand\";v=\"8\"",
            ["Sec-CH-UA-Platform"] = "\"Android\"",
            ["Sec-CH-UA-Platform-Version"] = "\"12.0.0\"",
        });

        var info = UAParser.GetClientInfo(userAgent, hints);

        Assert.Equal(new OS("HarmonyOS", "12"), info.OS);
        Assert.Equal("Huawei Browser", info.Browser.Name);
    }

    [Fact]
    public void A_generic_user_agent_is_corrected()
    {
        // An Android tablet asking for desktop sites sends a Linux user agent.
        const string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
        var hints = Hints(new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sec-CH-UA-Platform"] = "\"Android\"",
            ["Sec-CH-UA-Platform-Version"] = "\"14.0.0\"",
            ["Sec-CH-UA-Form-Factors"] = "\"Tablet\"",
        });

        var info = UAParser.GetClientInfo(userAgent, hints);

        Assert.Equal(new OS("Android", "14"), info.OS);
        Assert.Equal(DeviceTypes.Tablet, info.Device.Type);
    }

    [Theory]
    [InlineData("\"XR\"", DeviceTypes.Wearable)]
    [InlineData("\"Watch\"", DeviceTypes.Wearable)]
    [InlineData("\"Automotive\"", DeviceTypes.Embedded)]
    [InlineData("\"Mobile\", \"Tablet\"", DeviceTypes.Tablet)]
    public void Form_factors_give_the_device_type(string formFactors, string type)
    {
        var hints = Hints(With(Windows11, ("Sec-CH-UA-Form-Factors", formFactors)));

        Assert.Equal(type, UAParser.GetClientInfo(AndroidChrome, hints).Device.Type);
    }

    [Theory]
    [InlineData("\"Windows")]                   // no closing quote
    [InlineData("Windows")]                     // a token, not a string
    [InlineData("\"Windows\" x")]               // something after the string
    [InlineData("\"Winédows\"")]           // not ASCII
    public void Malformed_headers_are_ignored(string platform)
    {
        var hints = ClientHints.FromHeaders(name => name == "Sec-CH-UA-Platform" ? platform : name == "Sec-CH-UA-Mobile" ? "?1" : null);

        Assert.NotNull(hints);
        Assert.Null(hints.Platform);
        Assert.True(hints.Mobile);
    }

    [Theory]
    [InlineData("\"Chromium\";v=\"140\",")]                    // a trailing comma
    [InlineData("\"Chromium\";v=\"140\" \"Brave\";v=\"140\"")] // no comma
    [InlineData("Chromium;v=140")]                            // tokens
    public void Malformed_brand_lists_are_ignored(string brands)
    {
        var hints = ClientHints.FromHeaders(name => name == "Sec-CH-UA" ? brands : null);

        Assert.Null(hints);
    }

    [Fact]
    public void Reads_escapes_and_parameters()
    {
        var hints = ClientHints.FromHeaders(name => name == "Sec-CH-UA" ? "\"A \\\"quoted\\\" \\\\ brand\";x;v=\"1\";y=2, \"B\"" : null);

        Assert.Equal([new BrandVersion("A \"quoted\" \\ brand", "1")], hints!.Brands);
    }

    [Fact]
    public void Long_headers_are_ignored()
    {
        var model = "\"" + new string('a', ClientHints.MaxHeaderLength) + "\"";

        Assert.Null(ClientHints.FromHeaders(name => name == "Sec-CH-UA-Model" ? model : null));
    }

    [Fact]
    public void A_null_second_argument_still_means_no_renderer()
    {
        // Code written for 2.0 keeps compiling: (string?, string?) is chosen over (string?, ClientHints?, string? = null).
        Assert.Null(UAParser.GetClientInfo(WindowsChrome, null).GPU);
    }

    [Fact]
    public void No_hints_give_null()
    {
        Assert.Null(ClientHints.FromHeaders(_ => null));
        Assert.True(new ClientHints().IsEmpty);
        Assert.Equal(UAParser.GetClientInfo(WindowsChrome), UAParser.GetClientInfo(WindowsChrome, new ClientHints()));
    }

    [Fact]
    public void Values_that_are_not_versions_or_names_are_ignored()
    {
        var hints = new ClientHints
        {
            FullVersionList = [new("<script>", "1"), new("Google Chrome", "1<b>")],
            Platform = "Windows",
            PlatformVersion = "x",
            Model = "Pixel) evil (",
        };

        var info = UAParser.GetClientInfo(WindowsChrome, hints);

        Assert.Equal(UAParser.GetClientInfo(WindowsChrome).Browser, info.Browser);
        Assert.Equal(new OS("Windows", "10"), info.OS);
        Assert.Null(info.Device.Model);
    }

    [Fact]
    public void Hints_compare_by_value()
    {
        Assert.Equal(Hints(Windows11), Hints(Windows11));
        Assert.Equal(Hints(Windows11).GetHashCode(), Hints(Windows11).GetHashCode());
        Assert.NotEqual(Hints(Windows11), Hints(With(Windows11, ("Sec-CH-UA-Arch", "\"arm\""))));
    }

    [Fact]
    public void The_cache_keeps_results_per_user_agent_and_hints()
    {
        var cache = new ClientInfoCache();
        var windows11 = Hints(Windows11);
        var arm = Hints(With(Windows11, ("Sec-CH-UA-Arch", "\"arm\"")));

        var first = cache.GetClientInfo(WindowsChrome, windows11);

        Assert.Same(first, cache.GetClientInfo(WindowsChrome, Hints(Windows11)));
        Assert.Equal("arm64", cache.GetClientInfo(WindowsChrome, arm).CPU.Architecture);
        Assert.Equal("10", cache.GetClientInfo(WindowsChrome).OS.Version);
        Assert.Same(cache.GetClientInfo(WindowsChrome), cache.GetClientInfo(WindowsChrome, null));
        Assert.Equal(3, cache.Count);
    }

    private static async Task<(string Body, HttpResponseMessage Response)> RequestAsync(Action<UAParserOptions>? configure)
    {
        using var host = new HostBuilder()
            .ConfigureWebHost(web => web
                .UseTestServer()
                .ConfigureServices(s => s.AddRouting().AddUAParser(configure))
                .Configure(app => app
                    .UseUAParser()
                    .UseRouting()
                    .UseEndpoints(e => e.MapGet("/", (ClientInfo info) => $"{info.OS} | {info.CPU}"))))
            .Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.TryAddWithoutValidation("User-Agent", WindowsChrome);
        foreach (var (name, value) in Windows11)
            request.Headers.TryAddWithoutValidation(name, value);
        var response = await host.GetTestClient().SendAsync(request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), response);
    }

    [Fact]
    public async Task AspNetCore_reads_the_hints()
    {
        var (body, response) = await RequestAsync(null);

        Assert.Equal("OS     : Windows 11 | CPU    : amd64", body);
        Assert.False(response.Headers.Contains("Accept-CH"));
    }

    [Fact]
    public async Task AspNetCore_can_ignore_the_hints()
    {
        var (body, _) = await RequestAsync(o => o.UseClientHints = false);

        Assert.Equal("OS     : Windows 10 | CPU    : amd64", body);
    }

    [Fact]
    public async Task AspNetCore_can_ask_for_the_hints()
    {
        var (_, response) = await RequestAsync(o => o.RequestClientHints = true);

        Assert.Equal(string.Join(", ", ClientHints.HighEntropyHeaders), Assert.Single(response.Headers.GetValues("Accept-CH")));
    }

    [Fact]
    public void Several_header_lines_are_joined()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Sec-CH-UA-Full-Version-List"] = new Microsoft.Extensions.Primitives.StringValues(
            ["\"Chromium\";v=\"140.0.7339.128\"", "\"Google Chrome\";v=\"140.0.7339.128\""]);
        context.Request.Headers.UserAgent = WindowsChrome;

        var info = context.GetClientInfo();

        Assert.Equal(2, info.Hints!.FullVersionList.Count);
        Assert.Equal("140.0.7339.128", info.Browser.Version);
    }
}
