namespace uaParserLibrary.Models;

/// <summary>The browser engine, for example Blink, Gecko or WebKit.</summary>
/// <param name="Name">Engine name, or <see langword="null"/>.</param>
/// <param name="Version">Engine version, or <see langword="null"/>.</param>
public sealed record Engine(string? Name, string? Version)
{
    public override string ToString() => Describe.Line("Engine", Name, Version);
}
