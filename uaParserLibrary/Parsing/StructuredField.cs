using uaParserLibrary.Models;

namespace uaParserLibrary.Parsing;

// The parts of RFC 8941 (Structured Field Values for HTTP) that client hint headers use:
// strings ("Windows"), booleans (?1) and lists of strings with parameters
// ("Chromium";v="140", "Google Chrome";v="140"). Anything else, or anything malformed,
// gives no value: a header that cannot be read is ignored.
internal static class StructuredField
{
    // A single string: "Windows".
    public static string? String(string? value)
    {
        if (value is null)
            return null;
        var reader = new Reader(value);
        reader.SkipSpaces();
        if (!reader.TryString(out var text))
            return null;
        reader.SkipSpaces();
        return reader.AtEnd ? text : null;
    }

    // ?1 or ?0.
    public static bool? Boolean(string? value) => value?.Trim(' ', '\t') switch
    {
        "?1" => true,
        "?0" => false,
        _ => null,
    };

    // A list of strings: "Desktop", "XR". Parameters are ignored.
    public static IReadOnlyList<string> Strings(string? value) =>
        List(value) is { } items ? [.. items.Select(item => item.Value)] : [];

    // A list of strings with a "v" parameter: "Chromium";v="140", "Google Chrome";v="140".
    public static IReadOnlyList<BrandVersion> Brands(string? value) =>
        List(value) is { } items ? [.. items.Where(item => item.Version is not null).Select(item => new BrandVersion(item.Value, item.Version!))] : [];

    private static List<(string Value, string? Version)>? List(string? value)
    {
        if (value is null)
            return null;

        var items = new List<(string, string?)>();
        var reader = new Reader(value);
        reader.SkipSpaces();
        if (reader.AtEnd)
            return items;

        while (true)
        {
            if (!reader.TryString(out var item))
                return null;

            // Parameters: ;key or ;key=value. Only v="..." is kept.
            string? version = null;
            while (reader.Peek == ';')
            {
                reader.Next();
                reader.SkipSpaces();
                if (!reader.TryKey(out var key))
                    return null;
                if (reader.Peek != '=')
                    continue;   // a key alone means true
                reader.Next();
                if (reader.Peek == '"')
                {
                    if (!reader.TryString(out var text))
                        return null;
                    if (key == "v")
                        version = text;
                }
                else if (!reader.TryBareValue())
                {
                    return null;
                }
            }
            items.Add((item, version));

            reader.SkipSpaces();
            if (reader.AtEnd)
                return items;
            if (reader.Peek != ',')
                return null;
            reader.Next();
            reader.SkipSpaces();
            if (reader.AtEnd)
                return null;   // a trailing comma
        }
    }

    private ref struct Reader(string text)
    {
        private int _position;

        public readonly bool AtEnd => _position >= text.Length;

        public readonly char Peek => AtEnd ? '\0' : text[_position];

        public void Next() => _position++;

        public void SkipSpaces()
        {
            while (!AtEnd && text[_position] is ' ' or '\t')
                _position++;
        }

        // "..." with \" and \\ escapes; only printable ASCII is allowed.
        public bool TryString(out string value)
        {
            value = string.Empty;
            if (Peek != '"')
                return false;
            _position++;
            var builder = new System.Text.StringBuilder();
            while (!AtEnd)
            {
                var ch = text[_position++];
                if (ch == '"')
                {
                    value = builder.ToString();
                    return true;
                }
                if (ch == '\\')
                {
                    if (AtEnd || text[_position] is not ('"' or '\\'))
                        return false;
                    ch = text[_position++];
                }
                else if (ch is < ' ' or > '~')
                {
                    return false;
                }
                builder.Append(ch);
            }
            return false;   // no closing quote
        }

        // A parameter key: a lowercase letter or *, then lowercase letters, digits, _ - . *
        public bool TryKey(out string key)
        {
            var start = _position;
            if (Peek is not ((>= 'a' and <= 'z') or '*'))
            {
                key = string.Empty;
                return false;
            }
            while (!AtEnd && text[_position] is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '_' or '-' or '.' or '*')
                _position++;
            key = text[start.._position];
            return true;
        }

        // A token, number or boolean parameter value (not used by client hints; skipped).
        public bool TryBareValue()
        {
            var start = _position;
            while (!AtEnd && text[_position] is not (';' or ',' or ' ' or '\t' or '"'))
                _position++;
            return _position > start;
        }
    }
}
