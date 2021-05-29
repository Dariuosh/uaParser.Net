using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class CPUMatch
    {
        public IReadOnlyList<Regex> Regexes { get; init; }
        public Action<Match, CPU> Action { get; init; }
    }
}