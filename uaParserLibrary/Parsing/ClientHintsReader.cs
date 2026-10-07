using uaParserLibrary.Models;
using uaParserLibrary.Rules;

namespace uaParserLibrary.Parsing;

// Improves what the User-Agent string gives with User-Agent Client Hints. The hints fill in
// what the reduced User-Agent string of Chromium-based browsers hides (the full version, the
// Windows and macOS version, the CPU and the Android model) and name browsers that look like
// Chrome in it (Brave, for example). A value the User-Agent string gives more precisely, such as
// the name of an in-app browser or HarmonyOS, is kept.
internal static class ClientHintsReader
{
    // Browser names the User-Agent string gives for any Chromium-based browser that does not
    // name itself there; the brands can tell which one it is.
    private static readonly HashSet<string?> ChromeLike = [null, "Chrome", "Chromium"];

    // Brand -> browser name as the rules write it. Other brands are used as they are.
    private static readonly Dictionary<string, string> BrowserNames = new(StringComparer.Ordinal)
    {
        ["Google Chrome"] = "Chrome",
        ["Microsoft Edge"] = "Edge",
        ["Microsoft Edge WebView2"] = "Edge WebView2",
        ["Android WebView"] = "Chrome WebView",
        ["HeadlessChrome"] = "Chrome Headless",
        ["YaBrowser"] = "Yandex",
    };

    // Sec-CH-UA-Platform -> operating system name as the rules write it.
    private static readonly Dictionary<string, string> OsNames = new(StringComparer.Ordinal)
    {
        ["Windows"] = "Windows",
        ["macOS"] = "Mac OS",
        ["Android"] = "Android",
        ["Chrome OS"] = "Chromium OS",
        ["Chromium OS"] = "Chromium OS",
        ["Linux"] = "Linux",
        ["Fuchsia"] = "Fuchsia",
    };

    // Operating system names the User-Agent string gives when it cannot tell more.
    private static readonly HashSet<string?> GenericOs = [null, "Linux"];

    private const int MaxValueLength = 64;

    public static ClientInfo Apply(ClientInfo info, ClientHints hints)
    {
        var os = ReadOs(info.OS, hints);
        return info with
        {
            Browser = ReadBrowser(info.Browser, hints),
            Engine = ReadEngine(info.Engine, hints),
            OS = os,
            Device = ReadDevice(info.Device, hints, os),
            CPU = ReadCpu(info.CPU, hints),
            Hints = hints,
        };
    }

    // "GREASE" brands, made up so that sites do not rely on the brand list. The spec builds them
    // from words joined by any of ( ) - . / : ; = ? _ and spaces; Chromium uses the words
    // "Not A Brand" ("Not)A;Brand", "Not_A Brand", " Not A;Brand"...). No real brand uses those
    // characters, so a brand with one of them, or with "Not" and "Brand", is GREASE.
    public static bool IsGrease(string brand) =>
        brand.AsSpan().ContainsAny(GreaseCharacters) ||
        (brand.Contains("Not", StringComparison.Ordinal) &&
         (brand.Contains("Brand", StringComparison.Ordinal) || brand.Contains("Browser", StringComparison.Ordinal)));

    private static readonly System.Buffers.SearchValues<char> GreaseCharacters = System.Buffers.SearchValues.Create("()-./:;=?_");

    private static Browser ReadBrowser(Browser browser, ClientHints hints)
    {
        var full = hints.FullVersionList.Count > 0;
        var brands = Usable(full ? hints.FullVersionList : hints.Brands);
        if (brands.Count == 0)
            return browser;

        var best = Product(brands);
        var fullVersion = full ? null : ProductFullVersion(hints, best);
        string? Version(BrandVersion brand) =>
            full ? brand.Version
            : fullVersion is not null && brand == best ? fullVersion
            : browser.Version is null ? brand.Version : null;

        // A browser that looks like Chrome in the User-Agent string: the most specific brand
        // tells which one it is.
        if (ChromeLike.Contains(browser.Name))
        {
            var name = BrowserName(best.Brand);
            if (name != browser.Name)
            {
                var version = Version(best) ?? browser.Version;
                return new Browser(name, version, JsString.Majorize(version));
            }
        }

        // The same browser: only the version can be more precise.
        foreach (var brand in brands)
        {
            if (BrowserName(brand.Brand) == browser.Name && Version(brand) is { } version)
                return browser with { Version = version, Major = JsString.Majorize(version) };
        }
        return browser;
    }

    // The brand that names the product: Chromium itself last, Google Chrome before it, every
    // other brand (Edge, Opera, Brave...) first; the longest name wins ("Microsoft Edge WebView2"
    // over "Microsoft Edge").
    private static BrandVersion Product(List<BrandVersion> brands) =>
        brands.OrderBy(Rank).ThenByDescending(b => b.Brand.Length).ThenBy(b => b.Brand, StringComparer.Ordinal).First();

    // The deprecated Sec-CH-UA-Full-Version is the product's full version: used when its major
    // version is the product brand's.
    private static string? ProductFullVersion(ClientHints hints, BrandVersion product) =>
        IsVersion(hints.FullVersion) && JsString.Majorize(hints.FullVersion) == JsString.Majorize(product.Version)
            ? hints.FullVersion
            : null;

    private static int Rank(BrandVersion brand) => brand.Brand switch
    {
        "Chromium" => 2,
        "Google Chrome" => 1,
        _ => 0,
    };

    private static string BrowserName(string brand) => BrowserNames.GetValueOrDefault(brand, brand);

    private static Engine ReadEngine(Engine engine, ClientHints hints)
    {
        if (engine.Name is not (null or "Blink"))
            return engine;
        if (Usable(hints.FullVersionList).FirstOrDefault(b => b.Brand == "Chromium") is { } chromium)
            return new Engine("Blink", chromium.Version);

        // Without the full version list: Chrome's own full version is also Chromium's.
        var brands = Usable(hints.Brands);
        if (brands.FirstOrDefault(b => b.Brand == "Chromium") is not { } major)
            return engine;
        var product = Product(brands);
        if (product.Brand is "Google Chrome" or "Chromium" && ProductFullVersion(hints, product) is { } version)
            return new Engine("Blink", version);
        return engine.Name is null ? new Engine("Blink", major.Version) : engine;
    }

    private static OS ReadOs(OS os, ClientHints hints)
    {
        if (hints.Platform is null || !OsNames.TryGetValue(hints.Platform, out var name))
            return os;
        if (os.Name != name && !GenericOs.Contains(os.Name))
            return os;   // the User-Agent string names it more precisely (HarmonyOS, for example)

        var version = IsVersion(hints.PlatformVersion) ? hints.PlatformVersion : null;
        version = version is null ? null : name switch
        {
            "Windows" => WindowsVersion(version),
            "Mac OS" or "Android" => TrimZeros(version),
            // The spec gives Linux and Fuchsia no platform version; Chrome on Linux sends the
            // kernel's ("6.8.0"), which is not the version of the system.
            "Linux" or "Fuchsia" => null,
            _ => version,
        };
        return os.Name == name ? os with { Version = version ?? os.Version } : new OS(name, version);
    }

    // Sec-CH-UA-Platform-Version on Windows: 13 and later are Windows 11, 1 to 12 Windows 10,
    // 0.1 Windows 7, 0.2 Windows 8 and 0.3 Windows 8.1.
    private static string? WindowsVersion(string version)
    {
        var parts = version.Split('.');
        if (!int.TryParse(parts[0], out var major))
            return null;
        if (major >= 13) return "11";
        if (major >= 1) return "10";
        return parts.Length > 1 ? parts[1] switch { "1" => "7", "2" => "8", "3" => "8.1", _ => null } : null;
    }

    // "15.0.0" -> "15", "14.1.0" -> "14.1".
    private static string TrimZeros(string version)
    {
        while (version.EndsWith(".0", StringComparison.Ordinal))
            version = version[..^2];
        return version;
    }

    private static Device ReadDevice(Device device, ClientHints hints, OS os)
    {
        if (Model(hints.Model) is { } model)
            device = WithModel(device, model, hints, os);

        // The form factors the browser reports; "Desktop" changes nothing.
        foreach (var (formFactor, type) in FormFactorTypes)
        {
            if (hints.FormFactors.Contains(formFactor))
                return device with { Type = type };
        }
        if (hints.FormFactors.Count == 0 && hints.Mobile == true && device.Type is null)
            return device with { Type = DeviceTypes.Mobile };
        return device;
    }

    // Android models are read by the device rules, as if they were in an Android User-Agent
    // string, to find the vendor (for example "SM-S931B" is a Samsung phone). The rules are for
    // Android only, so other platforms (Windows can send "Surface Pro") keep just the model.
    private static Device WithModel(Device device, string model, ClientHints hints, OS os)
    {
        var android = hints.Platform == "Android" || (hints.Platform is null && os.Name == "Android");
        if (!android)
            return device with { Model = model };

        var mobile = hints.Mobile == false ? "" : "Mobile ";
        var userAgent = $"Mozilla/5.0 (Linux; Android {os.Version ?? "10"}; {model}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 {mobile}Safari/537.36";
        var values = Rule.Apply(UserAgentRules.Device, new Input(userAgent, UserAgentRules.PrefilterWords));
        return values[Field.Vendor] is { } vendor
            ? new Device(vendor, values[Field.Model] ?? model, values[Field.Type] ?? device.Type)
            : device with { Model = model };
    }

    private static readonly (string FormFactor, string Type)[] FormFactorTypes =
    [
        ("XR", DeviceTypes.Wearable),
        ("Watch", DeviceTypes.Wearable),
        ("Automotive", DeviceTypes.Embedded),
        ("Tablet", DeviceTypes.Tablet),
        ("Mobile", DeviceTypes.Mobile),
    ];

    // A model name that is safe to put in a User-Agent string: letters, digits, spaces and _ . + - /
    private static string? Model(string? model)
    {
        if (string.IsNullOrWhiteSpace(model) || model.Length > MaxValueLength)
            return null;
        foreach (var ch in model)
        {
            if (!(char.IsAsciiLetterOrDigit(ch) || ch is ' ' or '_' or '.' or '+' or '-' or '/'))
                return null;
        }
        return model.Trim();
    }

    private static CPU ReadCpu(CPU cpu, ClientHints hints)
    {
        var bits = hints.Bitness;
        return hints.Architecture switch
        {
            "x86" when bits == "64" => new CPU("amd64"),
            "x86" when bits == "32" => new CPU("ia32"),
            "x86" => cpu.Architecture is "amd64" or "ia32" ? cpu : new CPU("ia32"),
            "arm" when bits == "64" => new CPU("arm64"),
            "arm" when bits == "32" => cpu.Architecture is "arm" or "armhf" ? cpu : new CPU("arm"),
            "arm" => cpu.Architecture is "arm" or "armhf" or "arm64" ? cpu : new CPU("arm"),
            _ => cpu,
        };
    }

    // Brands that can be shown: a plain name (not GREASE), a version made of digits and dots.
    private static List<BrandVersion> Usable(IReadOnlyList<BrandVersion> brands)
    {
        var usable = new List<BrandVersion>(brands.Count);
        foreach (var brand in brands)
        {
            if (IsBrandName(brand.Brand) && !IsGrease(brand.Brand) && IsVersion(brand.Version))
                usable.Add(brand);
        }
        return usable;
    }

    private static bool IsVersion(string? version) =>
        version is { Length: > 0 and <= 32 } && char.IsAsciiDigit(version[0]) &&
        !version.AsSpan().ContainsAnyExcept("0123456789.");

    // Letters, digits, spaces and . _ - ("Google Chrome", "Microsoft Edge WebView2", "Opera GX");
    // brands with . _ - are GREASE (see IsGrease).
    private static bool IsBrandName(string? brand) =>
        brand is { Length: > 0 and <= MaxValueLength } && char.IsAsciiLetterOrDigit(brand[0]) &&
        !brand.AsSpan().ContainsAnyExcept(BrandCharacters);

    private static readonly System.Buffers.SearchValues<char> BrandCharacters =
        System.Buffers.SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ._-");
}
