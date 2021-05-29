using System.Collections.Generic;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

namespace uaParserLibrary.Expressions
{
    public static class CPUExpressions
    {
        public static IReadOnlyList<CPUMatch> Matches = new List<CPUMatch>
        {
            // AMD64 (x64)
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(?:(amd|x(?:(?:86|64)[-_])?|wow|win)64)[;\)]")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "amd64";
                }
            },
            // IA32 (quicktime) ,IA32 (x86)
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(ia32(?=;))"),
                    Util.CreateRegex(@"((?:i[346]|x)86)[;\)]")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "ia32";
                }
            },
            // ARM64
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(aarch64|arm(v?8e?l?|_?64))\b")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "arm64";
                }
            },
            // ARMHF
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(arm(?:v[67])?ht?n?[fl]p?)\b")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "armhf";
                }
            },
            // PocketPC mistakenly identified as PowerPC
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"windows (ce|mobile); ppc;"),
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "arm";
                }
            },
            // PowerPC
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"((?:ppc|powerpc)(?:64)?)(?: mac|;|\))")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "ppc";
                }
            },
            // SPARC
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(sun4\w)[;\)]")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = "sparc";
                }
            },
            // IA64, 68K, ARM/64, AVR/32, IRIX/64, MIPS/64, SPARC/64, PA-RISC
            new CPUMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(?<Arch>(?:avr32|ia64(?=;))|68k(?=\))|\barm(?=v(?:[1-7]|[5-7]1)l?|;|eabi)|(?=atmel )avr|(?:irix|mips|sparc)(?:64)?\b|pa-risc)")
                },
                Action = (Match match,CPU cpu) =>
                {
                    cpu.Architecture = match.Groups["Arch"].Value.ToLower();
                }
            }
        };
    }
}