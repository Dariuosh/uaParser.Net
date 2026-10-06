using System.Text;

namespace uaParserLibrary.Parsing;

// The text being parsed, shared by all rule sets of one parse. For all-ASCII text it keeps a
// lowercase copy and remembers which prefilter words it contains, so each word is searched for
// at most once. Ignoring ASCII case is exactly what the case-insensitive regexes do on ASCII
// text; for any other text prefilters are not used and every regex runs.
internal sealed class Input
{
    private readonly string[] _words;
    private readonly string? _lower;
    private sbyte[]? _seen;   // per word: 0 not checked yet, 1 present, -1 absent

    // words: the lowercase prefilter words the rules refer to by index.
    public Input(string text, string[] words)
    {
        Text = text;
        _words = words;
        if (Ascii.IsValid(text))
        {
            _lower = string.Create(text.Length, text, static (span, source) => Ascii.ToLower(source, span, out _));
        }
    }

    public string Text { get; }

    public bool CanPrefilter => _lower is not null;

    // True when the text contains the word, and always true when prefilters do not apply
    // (so a regex is never skipped by mistake).
    public bool Contains(int word)
    {
        if (_lower is null)
            return true;

        _seen ??= new sbyte[_words.Length];
        var state = _seen[word];
        if (state == 0)
        {
            state = _lower.Contains(_words[word], StringComparison.Ordinal) ? (sbyte)1 : (sbyte)-1;
            _seen[word] = state;
        }
        return state > 0;
    }
}
