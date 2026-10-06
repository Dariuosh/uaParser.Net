using uaParserDemoComponents.Results;

namespace uaParserDemoComponents.Playground;

/// <summary>
/// A run of the typed text and what it is: part of a value (Part and Field set), part of the text
/// a rule matched (MatchPart set), past the 500 characters the parser reads (Ignored), or plain.
/// </summary>
public sealed record Segment(string Text, string? Part, string? Field, string? MatchPart, bool Ignored);

/// <summary>Splits the text in the editor into segments, using the trace's real match positions.</summary>
public static class Highlight
{
    public static IReadOnlyList<Segment> Split(string text, ParseTrace trace)
    {
        if (text.Length == 0)
            return [];

        // The trace's positions refer to the parsed text: trimmed, at most 500 characters.
        var parsed = trace.UserAgent;
        var offset = parsed.Length == 0 ? 0 : text.IndexOf(parsed, StringComparison.Ordinal);
        if (offset < 0)
            return [new Segment(text, null, null, null, false)];

        var valueOwner = new (string Part, string Field)?[text.Length];
        var matchOwner = new string?[text.Length];

        // Parts in Explainer order (Browser, Engine, OS, Device, CPU): when two values share the
        // same characters (Chrome's version is also Blink's), the first part keeps them.
        foreach (var part in trace.Parts)
        {
            if (part.Hit is not { } hit)
                continue;

            for (var i = hit.Index; i < hit.Index + hit.Length; i++)
                matchOwner[offset + i] ??= part.Part;

            foreach (var value in hit.Values.Where(v => v.Index >= 0))
            {
                for (var i = value.Index; i < value.Index + value.Length; i++)
                    valueOwner[offset + i] ??= (part.Part, value.Field);
            }
        }

        var end = offset + parsed.Length;
        var segments = new List<Segment>();
        var start = 0;
        for (var i = 1; i <= text.Length; i++)
        {
            if (i < text.Length && valueOwner[i] == valueOwner[start] && matchOwner[i] == matchOwner[start] && (i >= end) == (start >= end))
                continue;

            var owner = valueOwner[start];
            var ignored = start >= end && !string.IsNullOrWhiteSpace(text[start..i]);
            segments.Add(new Segment(text[start..i], owner?.Part, owner?.Field, matchOwner[start], ignored));
            start = i;
        }

        return segments;
    }

    /// <summary>The CSS name of a part: "browser", "engine", "os", "device" or "cpu".</summary>
    public static string CssName(string part) => part.ToLowerInvariant();
}
