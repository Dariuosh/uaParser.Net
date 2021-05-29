using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class EngineMatch
    {
        public IReadOnlyList<Regex> Regexes { get; set; }
        public Action<Match, Engine> Action { get; set; }
    }
}