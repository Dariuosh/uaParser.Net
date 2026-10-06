using System.Text.RegularExpressions;

using uaParserLibrary.Parsing;

using static uaParserLibrary.Parsing.Assignment;

namespace uaParserLibrary.Rules;

// GPU rules for WebGL renderer strings. ua-parser-js 1.0.x has no GPU detection; these rules
// come from uaParser.Net 1.x and use the same engine and regex options as the generated rules.
internal static partial class GpuRules
{
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.ECMAScript;
    private const int TimeoutMilliseconds = Rule.TimeoutMilliseconds;

    public static readonly Rule[] All =
    [
        // Intel, NVIDIA, SiS
        new([Intel, Nvidia, Sis],
            [Capture(Field.Vendor), Capture(Field.Model)]),
        // ATI
        new([Radeon],
            [Capture(Field.Model), Constant(Field.Vendor, "ATI")]),
        // Qualcomm
        new([Adreno],
            [Replace(Field.Model, Trademark, "", all: true), Constant(Field.Vendor, "Qualcomm")]),
    ];

    [GeneratedRegex(@"(intel).*\b(hd\sgraphics\s\d{4}|iris(?:\spro)|gma\s\w+)", Options, TimeoutMilliseconds, "")]
    private static partial Regex Intel();

    [GeneratedRegex(@"(nvidia)\s(geforce\s(?:gtx?\s)\d\w+|quadro)", Options, TimeoutMilliseconds, "")]
    private static partial Regex Nvidia();

    [GeneratedRegex(@"(sis)\s(\w+)", Options, TimeoutMilliseconds, "")]
    private static partial Regex Sis();

    [GeneratedRegex(@"\b(radeon\shd\s\w{4,5})", Options, TimeoutMilliseconds, "")]
    private static partial Regex Radeon();

    [GeneratedRegex(@"(adreno\s(?:\(TM\)\s)\w+)", Options, TimeoutMilliseconds, "")]
    private static partial Regex Adreno();

    [GeneratedRegex(@"\(TM\)\s", Options, TimeoutMilliseconds, "")]
    private static partial Regex Trademark();
}
