using System.Collections.Generic;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

namespace uaParserLibrary.Expressions
{
    public static class GPUExpressions
    {
        private static void VendorModelAction(Match match, GPU gpu)
        {
            gpu.Model = match.Groups["Model"].Value;
            gpu.Vendor = match.Groups["Vendor"].Value;
        }

        public static IReadOnlyList<GPUMatch> Matches = new List<GPUMatch> {
            // Intel /NVIDIA /SiS
            new GPUMatch
            {
                Regexes = new List<Regex>
                {
                    // Intel
                    Util.CreateRegex(@"(?<Vendor>intel).*\b(?<Model>hd\sgraphics\s\d{4}|iris(?:\spro)|gma\s\w+)"),
                    // NVIDIA
                    Util.CreateRegex(@"(?<Vendor>nvidia)\s(?<Model>geforce\s(?:gtx?\s)\d\w+|quadro)") ,
                    // SiS
                    Util.CreateRegex(@"(?<Vendor>sis)\s(?<Model>\w+)")
                },
                Action = VendorModelAction
            },
            // ATI
            new GPUMatch
            {
                Regexes = new List<Regex>
                {
                    // Microsoft Edge
                    Util.CreateRegex(@"\b(?<Model>radeon\shd\s\w{4,5})")
                },
                Action = (Match match,GPU gpu) =>
                {
                    gpu.Model = match.Groups["Model"].Value;
                    gpu.Vendor = "ATI";
                }
            },
            // Qualcomm
            new GPUMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(?<Model>adreno\s(?:\(TM\)\s)\w+)")
                },
                Action = (Match match,GPU gpu) =>
                {
                    gpu.Model = Regex.Replace(match.Groups["Model"].Value,@"\(TM\)\s","");
                    gpu.Vendor = "Qualcomm";
                }
            },
        };
    }
}