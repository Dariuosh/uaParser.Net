using System.Text.RegularExpressions;

namespace uaParserLibrary.Parsing;

// One ua-parser-js rule: the regexes to try and how to set the values when one matches.
internal sealed class Rule
{
    // Safety net against pathological backtracking; normal user agents match in microseconds.
    public const int TimeoutMilliseconds = 250;

    // regexes: the [GeneratedRegex] methods. A regex object is only created the first time its
    // method is called (and then reused), so regexes that never run cost nothing at start-up.
    public Rule(Func<Regex>[] regexes, Assignment[] assignments, Prefilter[]? prefilters = null)
    {
        if (prefilters is not null && prefilters.Length != regexes.Length)
            throw new ArgumentException("One prefilter per regex is required.", nameof(prefilters));

        Regexes = regexes;
        Assignments = assignments;
        Prefilters = prefilters;
    }

    public Func<Regex>[] Regexes { get; }

    public Assignment[] Assignments { get; }

    // Optional, one per regex.
    public Prefilter[]? Prefilters { get; }

    // ua-parser-js's rgxMapper: the first regex that matches decides, and assignment p takes
    // capture group p + 1. Fields no assignment sets stay null. The values are returned by
    // value, so concurrent callers never share state.
    public static FieldValues Apply(Rule[] rules, Input input)
    {
        var values = new FieldValues();

        if (Find(rules, input) is { } found)
        {
            var assignments = rules[found.RuleIndex].Assignments;
            var groups = found.Match.Groups;
            for (var p = 0; p < assignments.Length; p++)
                values[assignments[p].Field] = assignments[p].Apply(groups[p + 1]);
        }

        return values;
    }

    // The first regex that matches, in rule order, or null when none does.
    public static RuleMatch? Find(Rule[] rules, Input input)
    {
        var prefilter = input.CanPrefilter;

        for (var i = 0; i < rules.Length; i++)
        {
            var rule = rules[i];
            for (var r = 0; r < rule.Regexes.Length; r++)
            {
                if (prefilter && rule.Prefilters is { } prefilters && !prefilters[r].Allows(input))
                    continue;

                Match match;
                try
                {
                    match = rule.Regexes[r]().Match(input.Text);
                }
                catch (RegexMatchTimeoutException)
                {
                    continue;
                }

                if (match.Success)
                    return new RuleMatch(i, r, match);
            }
        }

        return null;
    }
}

// The regex that decided a rule set's result: rules[RuleIndex].Regexes[RegexIndex].
internal readonly record struct RuleMatch(int RuleIndex, int RegexIndex, Match Match);
