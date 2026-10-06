namespace uaParserLibrary.Parsing;

// ua-parser-js's strMapper: returns the key of the first entry with a value that the input
// contains (case-insensitive); otherwise the default, if the map has one, or the input itself.
// Entries keep JavaScript's key order and an upstream "?" key is stored as a null result.
internal sealed class StringMap
{
    private readonly Entry[] _entries;
    private readonly bool _hasDefault;
    private readonly string? _default;

    public StringMap(Entry[] entries)
    {
        _entries = entries;
    }

    public StringMap(Entry[] entries, string? defaultValue)
    {
        _entries = entries;
        _hasDefault = true;
        _default = defaultValue;
    }

    public string? Map(string value)
    {
        var lower = value.ToLowerInvariant();
        foreach (var entry in _entries)
        {
            foreach (var needle in entry.LowerNeedles)
            {
                if (lower.Contains(needle, StringComparison.Ordinal))
                    return entry.Result;
            }
        }
        return _hasDefault ? _default : value;
    }

    internal sealed class Entry
    {
        public Entry(string? result, string[] needles)
        {
            Result = result;
            LowerNeedles = Array.ConvertAll(needles, n => n.ToLowerInvariant());
        }

        public string? Result { get; }

        public string[] LowerNeedles { get; }
    }
}
