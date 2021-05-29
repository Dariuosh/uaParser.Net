using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class OSMatch
    {
        public IReadOnlyList<Regex> Regexes { get; set; }
        public Action<Match, OS> Action { get; set; }
    }
}