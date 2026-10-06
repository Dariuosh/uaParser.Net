namespace uaParserLibrary.Parsing;

// String helpers that behave exactly like their ua-parser-js counterparts.
internal static class JsString
{
    // ua-parser-js parses at most this many characters (UA_MAX_LENGTH).
    public const int MaxUserAgentLength = 500;

    // What ua-parser-js's setUA does: no value means "", and a user agent longer than
    // the limit loses its leading whitespace and is cut to the limit.
    public static string NormalizeUserAgent(string? userAgent)
    {
        if (userAgent is null)
            return string.Empty;

        if (userAgent.Length <= MaxUserAgentLength)
            return userAgent;

        var trimmed = TrimStart(userAgent);
        return trimmed.Length <= MaxUserAgentLength ? trimmed : trimmed[..MaxUserAgentLength];
    }

    // str.replace(/^\s\s*/, '') with JavaScript's definition of \s.
    public static string TrimStart(string value)
    {
        var start = 0;
        while (start < value.Length && IsWhiteSpace(value[start]))
            start++;

        return value[start..];
    }

    // version.replace(/[^\d\.]/g, '').split('.')[0]
    public static string? Majorize(string? version)
    {
        if (version is null)
            return null;

        var end = version.IndexOf('.');
        var head = end < 0 ? version.AsSpan() : version.AsSpan(0, end);
        if (!head.ContainsAnyExceptInRange('0', '9'))
            return head.Length == version.Length ? version : head.ToString();

        Span<char> digits = stackalloc char[head.Length];
        var count = 0;
        foreach (var ch in head)
        {
            if (ch is >= '0' and <= '9')
                digits[count++] = ch;
        }
        return digits[..count].ToString();
    }

    // JavaScript's \s: ASCII whitespace plus the Unicode space separators, line separators and BOM.
    private static bool IsWhiteSpace(char ch) => (int)ch switch
    {
        0x09 or 0x0A or 0x0B or 0x0C or 0x0D or 0x20 or 0xA0 or 0x1680 => true,
        >= 0x2000 and <= 0x200A => true,
        0x2028 or 0x2029 or 0x202F or 0x205F or 0x3000 or 0xFEFF => true,
        _ => false,
    };
}
