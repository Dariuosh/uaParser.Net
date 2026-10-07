using uaParserLibrary.Parsing;

namespace uaParserLibrary.Models;

/// <summary>
/// User-Agent Client Hints: what Chromium-based browsers (Chrome, Edge, Opera, Brave, Samsung
/// Internet and others) tell about themselves besides the User-Agent header, in the
/// Sec-CH-UA-* request headers or through navigator.userAgentData in JavaScript. They give what
/// the reduced User-Agent string no longer does: the full browser version, Windows 11, the
/// macOS version, the CPU architecture and the Android device model.
/// </summary>
/// <remarks>
/// Browsers send <see cref="LowEntropyHeaders"/> to every secure (https) site.
/// <see cref="HighEntropyHeaders"/> are only sent to sites that ask for them in an Accept-CH
/// response header. Like the User-Agent header, hints are what a client says, not proof of what
/// it is.
/// </remarks>
public sealed record ClientHints
{
    /// <summary>Header values longer than this are ignored.</summary>
    public const int MaxHeaderLength = 500;

    /// <summary>The headers browsers send to every secure site.</summary>
    public static IReadOnlyList<string> LowEntropyHeaders { get; } = ["Sec-CH-UA", "Sec-CH-UA-Mobile", "Sec-CH-UA-Platform"];

    /// <summary>The headers browsers only send when a site asks for them (with an Accept-CH response header).</summary>
    public static IReadOnlyList<string> HighEntropyHeaders { get; } =
    [
        "Sec-CH-UA-Full-Version-List", "Sec-CH-UA-Platform-Version", "Sec-CH-UA-Model",
        "Sec-CH-UA-Arch", "Sec-CH-UA-Bitness", "Sec-CH-UA-Form-Factors", "Sec-CH-UA-WoW64",
    ];

    /// <summary>The browser's brands with their major versions (Sec-CH-UA), for example "Google Chrome" 140.</summary>
    public IReadOnlyList<BrandVersion> Brands { get; init; } = [];

    /// <summary>The brands with their full versions (Sec-CH-UA-Full-Version-List), for example "Google Chrome" 140.0.7339.128.</summary>
    public IReadOnlyList<BrandVersion> FullVersionList { get; init; } = [];

    /// <summary>
    /// The browser's full version (Sec-CH-UA-Full-Version), for example "140.0.7339.128", or
    /// <see langword="null"/>. Deprecated in favour of <see cref="FullVersionList"/>, which is used
    /// when both are present, but still sent to sites that ask for it.
    /// </summary>
    public string? FullVersion { get; init; }

    /// <summary>Whether the browser asks for the mobile version of sites (Sec-CH-UA-Mobile), or <see langword="null"/>.</summary>
    public bool? Mobile { get; init; }

    /// <summary>The operating system (Sec-CH-UA-Platform), for example "Windows", "macOS" or "Android", or <see langword="null"/>.</summary>
    public string? Platform { get; init; }

    /// <summary>The operating system version (Sec-CH-UA-Platform-Version), for example "15.0.0" (on Windows, 13 and later mean Windows 11), or <see langword="null"/>.</summary>
    public string? PlatformVersion { get; init; }

    /// <summary>The CPU architecture (Sec-CH-UA-Arch), for example "x86" or "arm", or <see langword="null"/>.</summary>
    public string? Architecture { get; init; }

    /// <summary>The CPU architecture's bitness (Sec-CH-UA-Bitness), for example "64", or <see langword="null"/>.</summary>
    public string? Bitness { get; init; }

    /// <summary>The device model (Sec-CH-UA-Model), for example "Pixel 9"; only Android sends one.</summary>
    public string? Model { get; init; }

    /// <summary>The device's form factors (Sec-CH-UA-Form-Factors), for example "Desktop", "Mobile", "Tablet", "XR" or "Watch".</summary>
    public IReadOnlyList<string> FormFactors { get; init; } = [];

    /// <summary>Whether a 32-bit browser runs on 64-bit Windows (Sec-CH-UA-WoW64), or <see langword="null"/>.</summary>
    public bool? Wow64 { get; init; }

    /// <summary>
    /// Reads the hints from request headers. Values that are not valid structured header values
    /// (RFC 8941), or longer than <see cref="MaxHeaderLength"/>, are ignored.
    /// </summary>
    /// <param name="header">Gives a header's value by name, or <see langword="null"/> when the request does not have it. When a request has the same header more than once, give the values joined with ", ".</param>
    /// <returns>The hints, or <see langword="null"/> when the request has none.</returns>
    public static ClientHints? FromHeaders(Func<string, string?> header)
    {
        ArgumentNullException.ThrowIfNull(header);

        string? Get(string name) => header(name) is { Length: > 0 and <= MaxHeaderLength } value ? value : null;
        string? Text(string name) => StructuredField.String(Get(name)) is { Length: > 0 } value ? value : null;

        var hints = new ClientHints
        {
            Brands = StructuredField.Brands(Get("Sec-CH-UA")),
            FullVersionList = StructuredField.Brands(Get("Sec-CH-UA-Full-Version-List")),
            FullVersion = Text("Sec-CH-UA-Full-Version"),
            Mobile = StructuredField.Boolean(Get("Sec-CH-UA-Mobile")),
            Platform = Text("Sec-CH-UA-Platform"),
            PlatformVersion = Text("Sec-CH-UA-Platform-Version"),
            Architecture = Text("Sec-CH-UA-Arch"),
            Bitness = Text("Sec-CH-UA-Bitness"),
            Model = Text("Sec-CH-UA-Model"),
            FormFactors = StructuredField.Strings(Get("Sec-CH-UA-Form-Factors")),
            Wow64 = StructuredField.Boolean(Get("Sec-CH-UA-WoW64")),
        };
        return hints.IsEmpty ? null : hints;
    }

    /// <summary>Whether no hint has a value.</summary>
    public bool IsEmpty =>
        Brands.Count == 0 && FullVersionList.Count == 0 && FullVersion is null && Mobile is null && Platform is null &&
        PlatformVersion is null && Architecture is null && Bitness is null && Model is null &&
        FormFactors.Count == 0 && Wow64 is null;

    /// <summary>Whether both have the same hints (the lists are compared item by item).</summary>
    public bool Equals(ClientHints? other) =>
        other is not null &&
        Brands.SequenceEqual(other.Brands) && FullVersionList.SequenceEqual(other.FullVersionList) &&
        FullVersion == other.FullVersion && Mobile == other.Mobile && Platform == other.Platform && PlatformVersion == other.PlatformVersion &&
        Architecture == other.Architecture && Bitness == other.Bitness && Model == other.Model &&
        FormFactors.SequenceEqual(other.FormFactors) && Wow64 == other.Wow64;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var brand in Brands) hash.Add(brand);
        foreach (var brand in FullVersionList) hash.Add(brand);
        hash.Add(FullVersion);
        hash.Add(Mobile);
        hash.Add(Platform);
        hash.Add(PlatformVersion);
        hash.Add(Architecture);
        hash.Add(Bitness);
        hash.Add(Model);
        foreach (var formFactor in FormFactors) hash.Add(formFactor);
        hash.Add(Wow64);
        return hash.ToHashCode();
    }

    /// <summary>A one-line description, for example "Hints  : Google Chrome 140.0.7339.128, Windows 19.0.0, x86 64".</summary>
    public override string ToString()
    {
        var brands = FullVersionList.Count > 0 ? FullVersionList : Brands;
        return Describe.Line("Hints", string.Join(", ", new[]
        {
            string.Join(", ", brands.Where(b => !ClientHintsReader.IsGrease(b.Brand)).Select(b => $"{b.Brand} {b.Version}")),
            FullVersionList.Count == 0 && FullVersion is not null ? $"full version {FullVersion}" : null,
            Join(Platform, PlatformVersion),
            Join(Architecture, Bitness),
            Model,
            string.Join(" ", FormFactors),
            Mobile is true ? "mobile" : null,
        }.Where(v => !string.IsNullOrEmpty(v))));
    }

    // Every value, each written as its length and the text, so different hints never give the
    // same key (ClientInfoCache).
    internal string CacheKey()
    {
        var key = new System.Text.StringBuilder();
        void Add(string? value) => key.Append(value?.Length ?? -1).Append(':').Append(value);
        void AddList(IReadOnlyList<BrandVersion> brands)
        {
            key.Append(brands.Count).Append('[');
            foreach (var brand in brands)
            {
                Add(brand.Brand);
                Add(brand.Version);
            }
        }

        AddList(Brands);
        AddList(FullVersionList);
        Add(FullVersion);
        Add(Mobile?.ToString());
        Add(Platform);
        Add(PlatformVersion);
        Add(Architecture);
        Add(Bitness);
        Add(Model);
        key.Append(FormFactors.Count).Append('[');
        foreach (var formFactor in FormFactors)
            Add(formFactor);
        Add(Wow64?.ToString());
        return key.ToString();
    }

    private static string? Join(string? a, string? b) => a is null ? b : b is null ? a : $"{a} {b}";
}

/// <summary>A brand and its version in <see cref="ClientHints.Brands"/> or <see cref="ClientHints.FullVersionList"/>.</summary>
/// <param name="Brand">For example "Google Chrome", "Microsoft Edge" or "Chromium".</param>
/// <param name="Version">For example "140" or "140.0.7339.128".</param>
public sealed record BrandVersion(string Brand, string Version);
