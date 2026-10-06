namespace uaParserLibrary.Parsing;

// A cheap test that lets the parser skip a regex: the regex can only match when the input
// contains, for every group, at least one of the group's words. Words are given by their index
// in the rule set's word table. tools/RuleGenerator works the words out from each regex and
// checks that they never reject a real match.
internal sealed class Prefilter
{
    private static readonly Prefilter Always = new([]);

    private readonly int[][] _groups;

    private Prefilter(int[][] groups)
    {
        _groups = groups;
    }

    public static Prefilter Needs(params int[][] groups) => groups.Length == 0 ? Always : new(groups);

    public bool Allows(Input input)
    {
        foreach (var words in _groups)
        {
            var found = false;
            foreach (var word in words)
            {
                if (input.Contains(word))
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
}
