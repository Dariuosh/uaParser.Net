using System.Text.RegularExpressions;

using uaParserLibrary.Rules;

namespace uaParserLibrary.Parsing;

// Why a user agent is read the way it is: for each part, the rule and regex that matched and
// where each value came from. It uses the same Rule.Find as the parser, so it always agrees
// with UAParser. Used by the demos and the tests; not part of the public API.
internal static class Explainer
{
    public static Explanation Explain(string? userAgent)
    {
        var input = new Input(JsString.NormalizeUserAgent(userAgent), UserAgentRules.PrefilterWords);

        return new Explanation(input.Text,
        [
            Explain("Browser", UserAgentRules.Browser, input),
            Explain("Engine", UserAgentRules.Engine, input),
            Explain("OS", UserAgentRules.Os, input),
            Explain("Device", UserAgentRules.Device, input),
            Explain("CPU", UserAgentRules.Cpu, input),
        ]);
    }

    private static PartExplanation Explain(string part, Rule[] rules, Input input)
    {
        if (Rule.Find(rules, input) is not { } found)
            return new PartExplanation(part, rules.Length, null);

        var rule = rules[found.RuleIndex];
        var regex = rule.Regexes[found.RegexIndex]();
        var groups = found.Match.Groups;
        var assignments = rule.Assignments;

        var values = new List<ValueSource>(assignments.Length);
        for (var p = 0; p < assignments.Length; p++)
        {
            var assignment = assignments[p];

            // As in Rule.Apply, a later assignment to the same field wins.
            if (Array.FindIndex(assignments, p + 1, a => a.Field == assignment.Field) >= 0)
                continue;

            var group = groups[p + 1];
            var fromText = assignment.ReadsCapture && group.Success && group.Length > 0;
            values.Add(new ValueSource(
                assignment.Field.ToString(),
                assignment.Apply(group),
                assignment.How,
                fromText ? group.Index : -1,
                fromText ? group.Length : 0));
        }

        var hit = new RuleHit(found.RuleIndex + 1, found.RegexIndex + 1, rule.Regexes.Length,
            Pattern(regex), found.Match.Index, found.Match.Length, values);
        return new PartExplanation(part, rules.Length, hit);
    }

    // In JavaScript notation, as in the ua-parser-js source.
    private static string Pattern(Regex regex) =>
        $"/{regex}/{((regex.Options & RegexOptions.IgnoreCase) != 0 ? "i" : "")}";
}

// UserAgent: the text that was parsed (trimmed, at most 500 characters); indexes refer to it.
internal sealed record Explanation(string UserAgent, IReadOnlyList<PartExplanation> Parts);

// Hit: null when no rule matched (every value of the part is null).
internal sealed record PartExplanation(string Part, int RuleCount, RuleHit? Hit);

// RuleNumber and RegexNumber count from 1. Index and Length: the whole match.
internal sealed record RuleHit(int RuleNumber, int RegexNumber, int RegexCount, string Pattern,
    int Index, int Length, IReadOnlyList<ValueSource> Values);

// Index is -1 when the value does not come from the user agent text (a constant).
internal sealed record ValueSource(string Field, string? Value, string How, int Index, int Length);
