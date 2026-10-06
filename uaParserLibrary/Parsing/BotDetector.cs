using System.Buffers;

using uaParserLibrary.Models;
using uaParserLibrary.Rules;

namespace uaParserLibrary.Parsing;

// Finds the bot a user agent belongs to with the rules in Rules/BotRules.g.cs. It gives exactly
// what detect in tools/RuleGenerator/rules/bot-rules.js gives: when several patterns match, the
// match that starts first wins, then the longest, then the pattern that comes first in the list.
// When none matches, a word such as "examplebot/1.0" still marks an unknown bot.
//
// Nearly all patterns are plain text, found with one multi-string search. The few regexes are
// skipped unless the user agent contains words every match must contain, so for a browser's
// user agent no regex runs at all.
internal static class BotDetector
{
    // A plain text pattern. AtStart and AtEnd: where it must be (^ and $ in the pattern).
    // (Classes, not structs, and no LINQ below: generic code over reference types is already
    // compiled in the framework, so building the tables costs little at start-up.)
    private sealed class Text(string value, int bot, bool atStart, bool atEnd)
    {
        public string Value { get; } = value;
        public int Bot { get; } = bot;
        public bool AtStart { get; } = atStart;
        public bool AtEnd { get; } = atEnd;
    }

    // A match: where it starts, how long it is, and the bot (a line of the table; -1 for none).
    private readonly struct Found(int index, int length, int bot)
    {
        public static readonly Found None = new(0, 0, -1);

        public int Index { get; } = index;
        public int Length { get; } = length;
        public int Bot { get; } = bot;

        public Found Or(Found other)
        {
            if (Bot < 0) return other;
            if (other.Index != Index) return other.Index < Index ? other : this;
            if (other.Length != Length) return other.Length > Length ? other : this;
            return other.Bot < Bot ? other : this;
        }
    }

    private static readonly Bot[] Bots;

    // The texts that can be anywhere: all of them in one search, and by first character
    // (longest first, then in list order) to tell which one was found. The texts are ASCII.
    private static readonly SearchValues<string> Anywhere;
    private static readonly Text[]?[] ByFirstChar = new Text[]?[128];

    // The texts that must be at the start or the end.
    private static readonly Text[] Anchored;

    // The first word group of every regex and of the fallback: a user agent that contains none
    // of these words cannot match any of them.
    private static readonly SearchValues<string> RegexWords;

    static BotDetector()
    {
        var lines = BotRules.Table.Split('\n');
        Bots = new Bot[lines.Length];
        var texts = new HashSet<string>(StringComparer.Ordinal);
        var byFirstChar = new List<Text>?[128];
        var anchored = new List<Text>();

        for (var i = 0; i < lines.Length; i++)
        {
            var fields = lines[i].TrimEnd('\r').Split('\t');
            Bots[i] = new Bot(fields[1], NullIfDash(fields[2]), NullIfDash(fields[3]));

            var find = fields[0];
            var value = find[1..];
            switch (find[0])
            {
                case '=':
                    texts.Add(value);
                    (byFirstChar[value[0]] ??= []).Add(new Text(value, i, false, false));
                    break;
                case '^': anchored.Add(new Text(value, i, true, false)); break;
                case '$': anchored.Add(new Text(value, i, false, true)); break;
                case '!': anchored.Add(new Text(value, i, true, true)); break;
                case '~': break;   // BotRules.Patterns
                default: throw new InvalidOperationException($"Unknown bot table entry \"{find}\".");
            }
        }

        Anywhere = SearchValues.Create([.. texts], StringComparison.Ordinal);
        for (var c = 0; c < byFirstChar.Length; c++)
        {
            if (byFirstChar[c] is not { } list)
                continue;
            var sorted = list.ToArray();
            Array.Sort(sorted, (a, b) => a.Value.Length != b.Value.Length ? b.Value.Length - a.Value.Length : a.Bot - b.Bot);
            ByFirstChar[c] = sorted;
        }
        Anchored = [.. anchored];

        var words = new HashSet<string>(StringComparer.Ordinal);
        foreach (var pattern in BotRules.Patterns)
            words.UnionWith(pattern.Needs[0]);
        words.UnionWith(BotRules.FallbackNeeds[0]);
        RegexWords = SearchValues.Create([.. words], StringComparison.OrdinalIgnoreCase);
    }

    public static Bot Detect(Input input)
    {
        var text = input.Text.AsSpan();
        var best = Found.None;

        var at = text.IndexOfAny(Anywhere);
        if (at >= 0 && text[at] < 128 && ByFirstChar[text[at]] is { } candidates)
        {
            foreach (var candidate in candidates)
            {
                if (text[at..].StartsWith(candidate.Value, StringComparison.Ordinal))
                {
                    best = new Found(at, candidate.Value.Length, candidate.Bot);
                    break;
                }
            }
        }

        foreach (var t in Anchored)
        {
            var found = t.AtStart && t.AtEnd ? text.SequenceEqual(t.Value)
                : t.AtStart ? text.StartsWith(t.Value, StringComparison.Ordinal)
                : text.EndsWith(t.Value, StringComparison.Ordinal);
            if (found)
                best = best.Or(new Found(t.AtStart ? 0 : text.Length - t.Value.Length, t.Value.Length, t.Bot));
        }

        // The words are compared ignoring ASCII case, which is only safe for all-ASCII text
        // (as for the browser rules, see Input); for any other text every regex runs.
        var prefilter = input.CanPrefilter;
        if (prefilter && !text.ContainsAny(RegexWords))
            return best.Bot < 0 ? Bot.None : Bots[best.Bot];

        foreach (var pattern in BotRules.Patterns)
        {
            if (prefilter && !Contains(text, pattern.Needs))
                continue;
            var match = pattern.Regex().Match(input.Text);
            if (match.Success)
                best = best.Or(new Found(match.Index, match.Length, pattern.Bot));
        }

        if (best.Bot >= 0)
            return Bots[best.Bot];

        if (prefilter && !Contains(text, BotRules.FallbackNeeds))
            return Bot.None;
        var unknown = BotRules.Fallback().Match(input.Text);
        return unknown.Success ? new Bot(unknown.Groups[1].Value, null, null) : Bot.None;
    }

    // For every group, the text contains at least one of its words (ignoring ASCII case).
    private static bool Contains(ReadOnlySpan<char> text, string[][] needs)
    {
        foreach (var group in needs)
        {
            var found = false;
            foreach (var word in group)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }

    private static string? NullIfDash(string value) => value == "-" ? null : value;
}
