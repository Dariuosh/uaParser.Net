using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class BrowserMatch
    {
        public IReadOnlyList<Regex> Regexes { get; set; }
        public Action<Match, Browser> Action { get; set; }
    }
}