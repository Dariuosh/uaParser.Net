namespace uaParserBlazorServerDemo.Components.Pages;

// The code shown on the home page.
internal static class HomeSnippets
{
    public const string ExampleUserAgent =
        "Mozilla/5.0 (iPhone; CPU iPhone OS 18_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.5 Mobile/15E148 Safari/604.1";

    public const string Program =
        """
        using uaParserMiddleware;

        builder.Services.AddUAParser();   // ClientInfo for each request
        // ...
        app.UseUAParser();                // optional: parse early
        """;

    public const string Page =
        """
        @inject ClientInfo Client

        <p>Hello, @Client.Browser.Name user on @Client.OS.Name!</p>

        // Minimal API
        app.MapGet("/hello", (ClientInfo client) => $"Hi, {client.Browser.Name}!");
        """;
}
