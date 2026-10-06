namespace uaParserLibrary.Models;

/// <summary>The operating system, for example Windows 10 or iOS 18.6.</summary>
/// <param name="Name">Operating system name, or <see langword="null"/>.</param>
/// <param name="Version">Version, or <see langword="null"/>.</param>
public sealed record OS(string? Name, string? Version)
{
    /// <summary>A one-line description, for example "Browser: Chrome 140.0.0.0".</summary>
    public override string ToString() => Describe.Line("OS", Name, Version);
}
