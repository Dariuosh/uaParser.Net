using System.Text.RegularExpressions;

using uaParserResource;

namespace uaParserLibrary
{
    public static class Util
    {
        public static string Majorize(string version)
        {
            return Regex.Replace(version, @"[^\d\.]", string.Empty).Split('.')[0];
        }

        // Like ua-parser-js: a group that took no part in the match is undefined,
        // while a group that matched an empty string stays empty.
        public static string GroupValue(Match match, string groupName)
        {
            var group = match.Groups[groupName];
            return group.Success ? group.Value : Keywords.Undefined;
        }

        public static string Trim(string str, int len = 255)
        {
            return Regex.Replace(str, @"^\s+|\s+$", string.Empty).Substring(0, len);
        }

        public static Regex CreateRegex(string pattern)
        {
            return new Regex(
                          pattern,
                              RegexOptions.Compiled
                            | RegexOptions.IgnoreCase
                            | RegexOptions.Singleline
                            | RegexOptions.CultureInvariant);
        }

    }
}