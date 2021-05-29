using System.Collections.Generic;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

namespace uaParserLibrary.Expressions
{
    public static class EngineExpressions
    {
        private static void NameVersionAction(Match match, Engine engine)
        {
            engine.Name = match.Groups["Name"].Value;
            engine.Version = match.Groups["Version"].Value;
        }

        public static IReadOnlyList<EngineMatch> Matches = new List<EngineMatch> {
            // EdgeHTML
            new EngineMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"windows.+ edge\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Engine engine) =>
                {
                    engine.Name = "Edge HTML";
                    engine.Version = match.Groups["Version"].Value;
                }
            },
            // Blink
            new EngineMatch{
                Regexes = new List<Regex>{
                    // Microsoft Edge
                    Util.CreateRegex(@"webkit\/537\.36.+chrome\/(?!27)(?<Version>[\w\.]+)")
                },
                Action = (Match match,Engine engine) =>{
                    engine.Name = "Blink";
                    engine.Version = match.Groups["Version"].Value;
                }
            },
            // Mix
            new EngineMatch{
                Regexes = new List<Regex>{
                    // Presto
                    Util.CreateRegex(@"(?<Name>presto)\/(?<Version>[\w\.]+)"),
                    // WebKit/Trident/NetFront/NetSurf/Amaya/Lynx/w3m/Goanna
                    Util.CreateRegex(@"(?<Name>webkit|trident|netfront|netsurf|amaya|lynx|w3m|goanna)\/(?<Version>[\w\.]+)"),
                    // Flow
                    Util.CreateRegex(@"ekioh(?<Name>flow)\/(?<Version>[\w\.]+)")  ,
                    // KHTML/Tasman/Links
                    Util.CreateRegex(@"(?<Name>khtml|tasman|links)[\/ ]\(?(?<Version>[\w\.]+)"),
                    // iCab
                    Util.CreateRegex(@"(?<Name>icab)[\/ ](?<Version>[23]\.[\d\.]+)")
                },
                Action = NameVersionAction
            },
            // Gecko
            new EngineMatch{
                Regexes = new List<Regex>{
                    // Opera mini on iphone >= 8.0
                    Util.CreateRegex(@"rv\:(?<Version>[\w\.]{1,9})\b.+(?<Name>gecko)")
                },
                Action = NameVersionAction
            }
        };
    }
}