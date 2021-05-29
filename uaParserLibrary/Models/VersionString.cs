using System;
using System.Linq;

namespace uaParserLibrary.Models
{
    internal static class VersionString
    {
        public static string Format(params string[] parts)
        {
            return string.Join(".", parts.Where(v => !String.IsNullOrEmpty(v)).ToArray());
        }
    }
}