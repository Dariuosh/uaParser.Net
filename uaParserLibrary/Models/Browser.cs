namespace uaParserLibrary.Models;

/// <summary>The browser that sent the user agent, for example Chrome 140.</summary>
/// <param name="Name">Browser name, or <see langword="null"/> when it is not detected.</param>
/// <param name="Version">Full version, or <see langword="null"/>.</param>
/// <param name="Major">Major version (the digits before the first dot), or <see langword="null"/>.</param>
public sealed record Browser(string? Name, string? Version, string? Major)
{
    public override string ToString() => Describe.Line("Browser", Name, Version);
}
