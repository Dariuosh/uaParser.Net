namespace uaParserLibrary.Models;

/// <summary>The processor architecture named in the user agent.</summary>
/// <param name="Architecture">For example "amd64", "arm64" or "ia32"; <see langword="null"/> when it is not detected.</param>
public sealed record CPU(string? Architecture)
{
    /// <summary>A one-line description, for example "CPU    : amd64".</summary>
    public override string ToString() => Describe.Line("CPU", Architecture);
}
