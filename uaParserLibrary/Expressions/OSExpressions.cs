using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

namespace uaParserLibrary.Expressions
{
    public static class OSExpressions
    {
        // Windows Version
        private static IReadOnlyList<(string key, string value)> WindowsVersion = new List<(string key, string value)>
        {
            ("4.90","ME"),
            ("NT3.51","NT 3.11"),
            ("NT4.0","NT 4.0"),
            ("NT 5.0","2000"),
            ("NT 5.1","XP"),
            ("NT 5.2","XP"),
            ("NT 6.0","Vista"),
            ("NT 6.1","7"),
            ("NT 6.2","8"),
            ("NT 6.3","8.1"),
            ("NT 6.4","10"),
            ("NT 10.0","10"),
            ("ARM","RT" )
        };

        private static void NameVersionAction(Match match, OS os)
        {
            os.Name = match.Groups["Name"].Value;
            os.Version = match.Groups["Version"].Value;
        }

        public static IReadOnlyList<OSMatch> Matches = new List<OSMatch> 
        {
            // Windows-based
            new OSMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"microsoft\s(?<Name>windows)\s(?<Version>vista|xp)")
                },
                Action = NameVersionAction
            },
            // Windows based
            new OSMatch
            {
                Regexes = new List<Regex>
                {
                    // Windows RT
                    Util.CreateRegex(@"(?<Name>windows) nt 6\.2; (?<Version>arm)"),
                    // Windows Phone
                    Util.CreateRegex(@"(?<Name>windows (?:phone(?: os)?|mobile))[\/ ]?(?<Version>[\d\.\w ]*)"),
                    Util.CreateRegex(@"(?<Name>windows)[\/ ]?(?<Version>[ntce\d\. ]+\w)(?!.+xbox)"),
                },
                Action = (Match match,OS os) =>
                {
                    os.Name = match.Groups["Name"].Value;

                    os.Version= WindowsVersion.FirstOrDefault(
                                    map=> match.Groups["Version"].Value.StartsWith(map.key)).value
                                ?? match.Groups["Version"].Value;
                }
            },
            // Windows based
            new OSMatch
            {
                Regexes = new List<Regex>
                {
                    // Windows RT
                    Util.CreateRegex(@"(?<Name>win(?=3|9|n)|win 9x )(?<Version>[nt\d\.]+)")
                },
                Action = (Match match,OS os) =>{
                    os.Name = "Windows";
                    os.Version= WindowsVersion.FirstOrDefault(
                                    map=> match.Groups["Version"].Value.StartsWith(map.key)).value
                                ??match.Groups["Version"].Value;
                }
            },
            // iOS/macOS
            new OSMatch{
                Regexes = new List<Regex>
                {
                    // Opera mini on iphone >= 8.0
                    Util.CreateRegex(@"ip[honead]{2,4}\b(?:.*os (?<Version>[\w]+) like mac|; opera)"),
                    Util.CreateRegex(@"cfnetwork\/.+darwin")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "iOS";
                    os.Version = match.Groups["Version"].Value.Replace('_','.');
                }
            },
            // Mac OS
            new OSMatch{
                Regexes = new List<Regex>
                {
                    // Opera mini on iphone >= 8.0
                    Util.CreateRegex(@"(mac os x) ?(?<Version>[\w\. ]*)"),
                    Util.CreateRegex(@"(macintosh|mac_powerpc\b)(?!.+haiku)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Mac OS";
                    os.Version = match.Groups["Version"].Value.Replace('_','.');
                }
            } ,
            // Mobile OSes - Android-x86
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"droid (?<Version>[\w\.]+)\b.+(?<Name>android[- ]x86)")
                },
                Action = NameVersionAction
            },
            // Android/WebOS/QNX/Bada/RIM/Maemo/MeeGo/Sailfish OS /Blackberry /Tizen/KaiOS /Series 40
            new OSMatch{
                Regexes = new List<Regex>
                {
                    // Android/WebOS/QNX/Bada/RIM/Maemo/MeeGo/Sailfish OS
                    Util.CreateRegex(@"(?<Name>android|webos|qnx|bada|rim tablet os|maemo|meego|sailfish)[-\/ ]?(?<Version>[\w\.]*)"),
                    // Blackberry
                    Util.CreateRegex(@"(?<Name>blackberry)\w*\/(?<Version>[\w\.]*)"),
                    // Tizen/KaiOS
                    Util.CreateRegex(@"(?<Name>tizen|kaios)[\/ ](?<Version>[\w\.]+)"),
                    // Series 40
                    Util.CreateRegex(@"\((?<Name>series40);")
                },
                Action = NameVersionAction
            } ,
            // BlackBerry 10
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\(bb(10);")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "BlackBerry";
                    os.Version = "10";
                }
            },
            // Symbian
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(?:symbian ?os|symbos|s60(?=;)|series60)[-\/ ]?(?<Version>[\w\.]*)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Symbian";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // Firefox OS
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"mozilla\/[\d\.]+ \((?:mobile|tablet|tv|mobile; [\w ]+); rv:.+ gecko\/(?<Version>[\w\.]+)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Firefox OS";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // WebOS
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"web0s;.+rt(?<Version>tv)"),
                    Util.CreateRegex(@"\b(?:hp)?wos(?:browser)?\/(?<Version>[\w\.]+)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "webOS";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // Google Chromecast
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"crkey\/(?<Version>[\d\.]+)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Chromecast";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // Chromium OS
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(cros) [\w]+ (?<Version>[\w\.]+\w)")
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Chromium OS";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // Solaris
            new OSMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(sunos) ?(?<Version>[\w\.\d]*)"),
                },
                Action = (Match match, OS os)  =>
                {
                    os.Name = "Solaris";
                    os.Version = match.Groups["Version"].Value;
                }
            },
            // // Console & Other
            new OSMatch{
                Regexes = new List<Regex>
                {
                    // Nintendo/Playstation
                    Util.CreateRegex(@"(?<Name>nintendo|playstation) (?<Version>[wids345portablevuch]+)"),
                    // Microsoft Xbox (360, One, X, S, Series X, Series S)
                    Util.CreateRegex(@"(?<Name>xbox);\s+xbox\s(?<Version>[^\);]+)") ,
                    // Joli/Palm
                    Util.CreateRegex(@"\b(?<Name>joli|palm)\b ?(?:os)?\/?(?<Version>[\w\.]*)") ,
                    // Mint
                    Util.CreateRegex(@"(?<Name>mint)[\/\(\) ]?(?<Version>\w*)") ,
                    // Mageia/VectorLinux
                    Util.CreateRegex(@"(?<Name>mageia|vectorlinux)[; ]") ,
                    // Ubuntu/Debian/SUSE/Gentoo/Arch/Slackware/Fedora/Mandriva/CentOS/PCLinuxOS/RedHat/Zenwalk/Linpus/Raspbian/Plan9/Minix/RISCOS/Contiki/Deepin/Manjaro/elementary/Sabayon/Linspire
                    Util.CreateRegex(@"(?<Name>[kxln]?ubuntu|debian|suse|opensuse|gentoo|arch(?= linux)|slackware|fedora|mandriva|centos|pclinuxos|red ?hat|zenwalk|linpus|raspbian|plan 9|minix|risc os|contiki|deepin|manjaro|elementary os|sabayon|linspire)(?: gnu\/linux)?(?: enterprise)?(?:[- ]linux)?(?:-gnu)?[-\/ ]?(?!chrom|package)(?<Version>[-\w\.]*)") ,

                    // Hurd/Linux
                    Util.CreateRegex(@"(?<Name>hurd|linux) ?(?<Version>[\w\.]*)") ,
                    // GNU
                    Util.CreateRegex(@"(?<Name>gnu) ?(?<Version>[\w\.]*)") ,
                    // FreeBSD/NetBSD/OpenBSD/PC-BSD/GhostBSD/DragonFly
                    Util.CreateRegex(@"\b(?<Name>[-frentopcghs]{0,5}bsd|dragonfly)[\/ ]?(?!amd|[ix346]{1,2}86)(?<Version>[\w\.]*)"),
                    // Haiku
                    Util.CreateRegex(@"(?<Name>haiku) (?<Version>\w+)"),
                    // Solaris
                    Util.CreateRegex(@"(?<Name>(?:open)?solaris)[-\/ ]?(?<Version>[\w\.]*)"),
                    // AIX
                    Util.CreateRegex(@"(?<Name>aix) (?<Version>(\d)(?=\.|\)| )[\w\.])*"),
                    // BeOS/OS2/AmigaOS/MorphOS/OpenVMS/Fuchsia/HP-UX
                    Util.CreateRegex(@"\b(?<Name>beos|os\/2|amigaos|morphos|openvms|fuchsia|hp-ux)"),
                    // UNIX
                    Util.CreateRegex(@"(?<Name>unix) ?(?<Version>[\w\.]*)")
                },
                Action = NameVersionAction
            }
        };
    }
}