using uaParserLibrary.Parsing;

namespace uaParserDemoComponents.Results;

/// <summary>
/// Why a user agent is read the way it is: for each part, the rule that matched and where each
/// value came from. A public copy of the library's internal Explainer result, which the demos
/// see through InternalsVisibleTo; it is not part of the uaParser.Net API.
/// </summary>
/// <param name="UserAgent">The text that was parsed (trimmed, at most 500 characters); positions refer to it.</param>
public sealed record ParseTrace(string UserAgent, IReadOnlyList<PartTrace> Parts)
{
    public static ParseTrace Of(string? userAgent)
    {
        var explanation = Explainer.Explain(userAgent);
        return new ParseTrace(explanation.UserAgent, [.. explanation.Parts.Select(Copy)]);
    }

    public PartTrace? this[string part] => Parts.FirstOrDefault(p => p.Part == part);

    private static PartTrace Copy(PartExplanation part) =>
        new(part.Part, part.RuleCount, part.Hit is not { } hit ? null : new RuleTrace(
            hit.RuleNumber, hit.RegexNumber, hit.RegexCount, hit.Pattern, hit.Index, hit.Length,
            [.. hit.Values.Select(v => new ValueTrace(v.Field, v.Value, v.How, v.Index, v.Length))]));
}

/// <param name="Part">Browser, Engine, OS, Device or CPU.</param>
/// <param name="Hit">Null when none of the part's rules matched.</param>
public sealed record PartTrace(string Part, int RuleCount, RuleTrace? Hit);

/// <param name="RuleNumber">Counts from 1, in the order ua-parser-js tries the rules.</param>
/// <param name="Pattern">The regex that matched, in JavaScript notation.</param>
/// <param name="Index">Where the match starts.</param>
public sealed record RuleTrace(int RuleNumber, int RegexNumber, int RegexCount, string Pattern,
    int Index, int Length, IReadOnlyList<ValueTrace> Values);

/// <param name="Index">Where the value was read, or -1 when the rule sets it (a constant).</param>
public sealed record ValueTrace(string Field, string? Value, string How, int Index, int Length);
