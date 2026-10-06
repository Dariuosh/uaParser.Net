using uaParserBlazorServerDemo;
using uaParserBlazorServerDemo.Components;
using uaParserDemoComponents.Interop;
using uaParserLibrary;
using uaParserMiddleware;
using uaParserSamples;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    // The log analyzer receives pasted text in one message (the default limit is 32 KB).
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 1024 * 1024);

// uaParser.Net: ClientInfo for the current request (scoped), HttpContext.GetClientInfo() and a
// shared ClientInfoCache for repeated user agents.
builder.Services.AddUAParser();

builder.Services.AddScoped<DemoJs>();

var app = builder.Build();

// Optional: create the regexes the common user agents need now, rather than during the first
// requests (uaParser.Net creates each regex the first time it is needed).
_ = Task.Run(() =>
{
    foreach (var sample in SampleUserAgents.All)
        UAParser.GetClientInfo(sample.UserAgent);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseClientHints();
app.UseUAParser();

app.UseAntiforgery();
app.MapStaticAssets();

// A small JSON API.
app.MapGet("/api/client", (HttpContext context) => context.GetClientInfo());
app.MapGet("/api/parse", (string? ua, ClientInfoCache cache) => cache.GetClientInfo(ua));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
