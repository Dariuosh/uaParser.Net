using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class GPUMatch
    {
        public IReadOnlyList<Regex> Regexes { get; set; }
        public Action<Match, GPU> Action { get; set; }
    }
}