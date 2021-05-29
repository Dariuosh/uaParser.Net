using System.Text.RegularExpressions;

namespace uaParserLibrary
{
    public static class Util
    {
        public static string Majorize(string version)
        {
            return Regex.Replace(version, @"[^\d\.]", string.Empty).Split('.')[0];
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