using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uaParserLibrary.Models
{
    public class DeviceMatch
    {
        public IReadOnlyList<Regex> Regexes { get; init; }
        public Action<Match, Device> Action { get; init; }
    }
}