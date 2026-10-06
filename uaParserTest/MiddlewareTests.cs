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

// The middleware in a real ASP.NET Core pipeline (TestServer).
public class MiddlewareTests
{
    private const string Chrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
    private const string IPhone =
        "Mozilla/5.0 (iPhone; CPU iPhone OS 18_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.6 Mobile/15E148 Safari/604.1";

    private static async Task<IHost> StartAsync(Action<IServiceCollection> services, Action<IApplicationBuilder> pipeline)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(web => web
                .UseTestServer()
                .ConfigureServices(s => { s.AddRouting(); services(s); })
                .Configure(pipeline))
            .Build();
        await host.StartAsync(TestContext.Current.CancellationToken);
        return host;
    }

    private static async Task<string> GetAsync(IHost host, string path, string userAgent)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
        var response = await host.GetTestClient().SendAsync(request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    }

    // Endpoint that reports the injected ClientInfo.
    private static void InjectedEndpoint(IApplicationBuilder app) => app
        .UseUAParser()
        .UseRouting()
        .UseEndpoints(e => e.MapGet("/", (ClientInfo info) => $"{info.Browser.Name}|{info.OS.Name}"));

    [Fact]
    public async Task Each_request_gets_its_own_user_agent()
    {
        using var host = await StartAsync(s => s.AddUAParser(), InjectedEndpoint);

        var results = await Task.WhenAll(Enumerable.Range(0, 200).Select(i =>
            GetAsync(host, "/", i % 2 == 0 ? Chrome : IPhone)));

        for (var i = 0; i < results.Length; i++)
            Assert.Equal(i % 2 == 0 ? "Chrome|Windows" : "Mobile Safari|iOS", results[i]);
    }

    [Fact]
    public async Task Works_with_the_cache_turned_off()
    {
        using var host = await StartAsync(s => s.AddUAParser(o => o.CacheCapacity = 0), InjectedEndpoint);

        Assert.Equal("Mobile Safari|iOS", await GetAsync(host, "/", IPhone));
        Assert.Null(host.Services.GetService<ClientInfoCache>());
    }

    [Fact]
    public async Task Repeated_user_agents_come_from_the_shared_cache()
    {
        using var host = await StartAsync(s => s.AddUAParser(o => o.CacheCapacity = 10), InjectedEndpoint);

        await GetAsync(host, "/", Chrome);
        await GetAsync(host, "/", Chrome);
        await GetAsync(host, "/", IPhone);

        Assert.Equal(2, host.Services.GetRequiredService<ClientInfoCache>().Count);
    }

    [Fact]
    public async Task The_middleware_parses_before_the_rest_of_the_pipeline()
    {
        using var host = await StartAsync(s => s.AddUAParser(), app => app
            .UseUAParser()
            .Run(context => context.Response.WriteAsync(
                context.Items.Values.OfType<ClientInfo>().SingleOrDefault()?.Browser.Name ?? "not parsed yet")));

        Assert.Equal("Chrome", await GetAsync(host, "/", Chrome));
    }

    [Fact]
    public async Task GetClientInfo_works_without_the_middleware_and_without_AddUAParser()
    {
        using var host = await StartAsync(_ => { }, app => app
            .Run(context => context.Response.WriteAsync(context.GetClientInfo().OS.Name ?? "")));

        Assert.Equal("iOS", await GetAsync(host, "/", IPhone));
    }

    [Fact]
    public void Outside_a_request_the_values_are_null()
    {
        using var provider = new ServiceCollection().AddUAParser().BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.Equal(UAParser.GetClientInfo(null), scope.ServiceProvider.GetRequiredService<ClientInfo>());
    }

    [Fact]
    public void Negative_cache_capacity_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ServiceCollection().AddUAParser(o => o.CacheCapacity = -1));
    }
}
