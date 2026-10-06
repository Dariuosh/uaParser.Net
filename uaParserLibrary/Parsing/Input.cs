using System.Text;

namespace uaParserLibrary.Parsing;

// The text being parsed, shared by all rule sets of one parse. For all-ASCII text it remembers
// which prefilter words the text contains, so each word is searched for at most once. Ignoring
// ASCII case is exactly what the case-insensitive regexes do on ASCII text; for any other text
// prefilters are not used and every regex runs.
internal sealed class Input
{
    private readonly string[] _words;
    private ulong[]? _bits;   // two bits per word: checked, present

    // words: the lowercase prefilter words the rules refer to by index.
    public Input(string text, string[] words)
    {
        Text = text;
        _words = words;
        CanPrefilter = Ascii.IsValid(text);
    }

    public string Text { get; }

    public bool CanPrefilter { get; }

    // True when the text contains the word, and always true when prefilters do not apply
    // (so a regex is never skipped by mistake).
    public bool Contains(int word)
    {
        if (!CanPrefilter)
            return true;

        _bits ??= new ulong[(_words.Length + 31) / 32];
        var slot = word >> 5;
        var shift = (word & 31) * 2;
        var state = (_bits[slot] >> shift) & 3;
        if (state == 0)
        {
            state = Text.Contains(_words[word], StringComparison.OrdinalIgnoreCase) ? 3ul : 1ul;
            _bits[slot] |= state << shift;
        }
        return state == 3;
    }
}
