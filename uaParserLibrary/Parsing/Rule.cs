using System.Text.RegularExpressions;

namespace uaParserLibrary.Parsing;

// One ua-parser-js rule: the regexes to try and how to set the values when one matches.
internal sealed class Rule
{
    // Safety net against pathological backtracking; normal user agents match in microseconds.
    public const int TimeoutMilliseconds = 250;

    public Rule(Regex[] regexes, Assignment[] assignments, Prefilter[]? prefilters = null)
    {
        if (prefilters is not null && prefilters.Length != regexes.Length)
            throw new ArgumentException("One prefilter per regex is required.", nameof(prefilters));

        Regexes = regexes;
        Assignments = assignments;
        Prefilters = prefilters;
    }

    public Regex[] Regexes { get; }

    public Assignment[] Assignments { get; }

    // Optional, one per regex.
    public Prefilter[]? Prefilters { get; }

    // ua-parser-js's rgxMapper: the first regex that matches decides, and assignment p takes
    // capture group p + 1. Fields no assignment sets stay null. A fresh array is returned on
    // every call, so concurrent callers never share state.
    public static string?[] Apply(Rule[] rules, Input input)
    {
        var values = new string?[Fields.Count];
        var prefilter = input.CanPrefilter;

        foreach (var rule in rules)
        {
            for (var r = 0; r < rule.Regexes.Length; r++)
            {
                if (prefilter && rule.Prefilters is { } prefilters && !prefilters[r].Allows(input))
                    continue;

                Match match;
                try
                {
                    match = rule.Regexes[r].Match(input.Text);
                }
                catch (RegexMatchTimeoutException)
                {
                    continue;
                }

                if (!match.Success)
                    continue;

                var assignments = rule.Assignments;
                for (var p = 0; p < assignments.Length; p++)
                    values[(int)assignments[p].Field] = assignments[p].Apply(match.Groups[p + 1]);

                return values;
            }
        }

        return values;
    }
}
