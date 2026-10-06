using System.Text.Json;

using Microsoft.JSInterop;

namespace uaParserDemoComponents.Interop;

/// <summary>Calls demo.js. Register it as a scoped service; only call it once the page is interactive.</summary>
public sealed class DemoJs(IJSRuntime js) : IAsyncDisposable
{
    private Task<IJSObjectReference>? _module;

    private Task<IJSObjectReference> Module =>
        _module ??= js.InvokeAsync<IJSObjectReference>("import", "./_content/uaParserDemoComponents/demo.js").AsTask();

    public async ValueTask<string> UserAgentAsync() =>
        await (await Module).InvokeAsync<string>("userAgent");

    public async ValueTask<int> MaxTouchPointsAsync() =>
        await (await Module).InvokeAsync<int>("maxTouchPoints");

    /// <summary>The browser's User-Agent Client Hints, or null when it has none (Firefox, Safari).</summary>
    public async ValueTask<ClientHints?> ClientHintsAsync()
    {
        // Passed as JSON and read with the source-generated serializer, which trimming keeps intact.
        var json = await (await Module).InvokeAsync<string?>("clientHints");
        return json is null ? null : JsonSerializer.Deserialize(json, DemoJson.Default.ClientHints);
    }

    public async ValueTask<string?> WebGLRendererAsync() =>
        await (await Module).InvokeAsync<string?>("webglRenderer");

    public async ValueTask DownloadAsync(string fileName, string contentType, Stream content)
    {
        using var reference = new DotNetStreamReference(content);
        await (await Module).InvokeVoidAsync("downloadStream", fileName, contentType, reference);
    }

    public async ValueTask ScrollToAsync(string id) =>
        await (await Module).InvokeVoidAsync("scrollToId", id);

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        try
        {
            await (await _module).DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone (Blazor Server); nothing to release.
        }
    }
}

/// <summary>What navigator.userAgentData reports. High-entropy values are null when the browser withholds them.</summary>
public sealed record ClientHints(
    IReadOnlyList<BrandVersion> Brands,
    bool Mobile,
    string Platform,
    string? PlatformVersion,
    string? Architecture,
    string? Bitness,
    string? Model,
    bool? Wow64,
    IReadOnlyList<BrandVersion>? FullVersionList,
    IReadOnlyList<string>? FormFactors);

/// <summary>One entry of a brand list, such as "Google Chrome" "131".</summary>
public sealed record BrandVersion(string Brand, string Version);
