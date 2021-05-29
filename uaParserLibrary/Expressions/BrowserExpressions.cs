using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

namespace uaParserLibrary.Expressions
{
    public static class BrowserExpressions
    {
        // Safari < 3.0
        private static IReadOnlyList<(string key, string value)> oldSafariMap = new List<(string key, string value)>
        {
            ("8","1.0"),
            ("1","1.2"),
            ("3","1.3"),
            ("412","2.0"),
            ("416","2.0.2"),
            ("417","2.0.3"),
            ("419","2.0.4"),
            ("?","/" )
        };

        private static void NameVersionAction(Match match, Browser browser)
        {
            browser.Name = match.Groups["Name"].Value;
            browser.Version = match.Groups["Version"].Value;
        }

        public static IReadOnlyList<BrowserMatch> Matches = new List<BrowserMatch> {
            // Chrome for Android/iOS
            new BrowserMatch{                
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"\b(?:crmo|crios)\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>
                {
                    browser.Name = "Chrome";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Edge
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Microsoft Edge
                    Util.CreateRegex(@"edg(?:e|ios|a)?\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Edge";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Presto based
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Opera Mini
                    Util.CreateRegex(@"(?<Name>opera mini)\/(?<Version>[-\w\.]+)"),
                    // Opera Mobi/Tablet
                    Util.CreateRegex(@"(?<Name>opera [mobiletab]{3,6})\b.+version\/(?<Version>[-\w\.]+)"),
                    // Opera
                    Util.CreateRegex(@"(?<Name>opera)(?:.+version\/|[\/ ]+)(?<Version>[\w\.]+)")
                },
                Action = NameVersionAction
            },
            // Opera Mini
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Opera mini on iphone >= 8.0
                    Util.CreateRegex(@"opios[\/ ]+(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Opera Mini";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Opera
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Opera Webkit
                    Util.CreateRegex(@"\bopr\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Opera";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Mixed, Trident based, Webkit/KHTML based
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Mixed

                    // Kindle
                    Util.CreateRegex(@"(?<Name>kindle)\/(?<Version>[\w\.]+)"),
                    // Lunascape/Maxthon/Netfront/Jasmine/Blazer
                    Util.CreateRegex(@"(?<Name>lunascape|maxthon|netfront|jasmine|blazer)[\/ ]?(?<Version>[\w\.]*)"),

                    // Trident based

                    // Avant/IEMobile/SlimBrowser
                    Util.CreateRegex(@"(?<Name>avant |iemobile|slim)(?:browser)?[\/ ]?(?<Version>[\w\.]*)"),
                    // Baidu Browser
                    Util.CreateRegex(@"(?<Name>ba?idubrowser)[\/ ]?(?<Version>[\w\.]+)"),
                    // Internet Explorer
                    Util.CreateRegex(@"(?:ms|\()(?<Name>ie) (?<Version>[\w\.]+)"),

                    // Webkit/KHTML based

                    // Flock/RockMelt/Midori/Epiphany/Silk/Skyfire/Bolt/Iron/Iridium/PhantomJS/Bowser/QupZilla/Falkon
                    // Rekonq/Puffin/Brave/Whale/QQBrowserLite/QQ, aka ShouQ
                    Util.CreateRegex(@"(?<Name>flock|rockmelt|midori|epiphany|silk|skyfire|ovibrowser|bolt|iron|vivaldi|iridium|phantomjs|bowser|quark|qupzilla|falkon|rekonq|puffin|brave|whale|qqbrowserlite|qq)\/(?<Version>[-\w\.]+)"),
                    // Weibo
                    Util.CreateRegex(@"(?<Name>weibo)__(?<Version>[\d\.]+)"),
                },
                Action = NameVersionAction
            },
            // UCBrowser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // UCBrowser
                    Util.CreateRegex(@"(?:\buc? ?browser|(?:juc.+)ucweb)[\/ ]?(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "UCBrowser";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // WeChat(Win) Desktop
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // WeChat Desktop for Windows Built-in Browser
                    Util.CreateRegex(@"\bqbcore\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "WeChat(Win) Desktop";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // WeChat
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // WeChat
                    Util.CreateRegex(@"micromessenger\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "WeChat";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Konqueror
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Konqueror
                    Util.CreateRegex(@"konqueror\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Konqueror";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // IE
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // IE11
                    Util.CreateRegex(@"trident.+rv[: ](?<Version>[\w\.]{1,9})\b.+like gecko")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "IE";
                    browser.Version =match.Groups["Version"].Value;
                }
            },
            // Yandex
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Yandex
                    Util.CreateRegex(@"yabrowser\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Yandex";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Avast/AVG Secure Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Avast/AVG Secure Browser
                    Util.CreateRegex(@"(?<Name>avast|avg)\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = $"{match.Groups["Name"]} Secure Browser";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Firefox Focus
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Firefox Focus
                    Util.CreateRegex(@"\bfocus\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Firefox Focus";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Opera Touch
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Opera Touch
                    Util.CreateRegex(@"\bopt\/(?<Version>[\w\.]+)$")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Opera Touch";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Coc Coc Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Coc Coc Browser
                    Util.CreateRegex(@"coc_coc\w+\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Coc Coc";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Dolphin
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Dolphin
                    Util.CreateRegex(@"dolfin\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Dolphin";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Opera Coast
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Opera Coast
                    Util.CreateRegex(@"coast\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Opera Coast";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // MIUI Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"miuibrowser\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "MIUI Browser";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Firefox for iOS
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"fxios\/(?<Version>[-\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Firefox";
                    browser.Version =  match.Groups["Version"].Value;
                }
            },
            // 360 Browser ==> Check
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"\bqihu|(qi?ho?o?|360)browser"),
                  // Util.CreateRegex(@"(qi?ho?o?|360)browser\/([\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "360 Browser";
                    browser.Version = string.Empty;
                }
            },
            // Oculus/Samsung/Sailfish Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Microsoft Edge
                    Util.CreateRegex(@"(?<Name>oculus|samsung|sailfish)browser\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = $"{match.Groups["Name"]} Browser";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Comodo Dragon
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"(?<Name>comodo_dragon)\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = match.Groups["Name"].Value.Replace('_',' ');
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Electron-based App /Tesl /QQBrowser/Baidu App/2345 Browser /SouGouBrowser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Electron-based App
                    Util.CreateRegex(@"(?<Name>electron)\/(?<Version>[\w\.]+)"),
                    // Tesl
                    Util.CreateRegex(@"(?<Name>tesla)(?: qtcarbrowser|\/(?<Version>20\d\d\.[-\w\.]+))"),
                    // QQBrowser/Baidu App/2345 Browser
                    Util.CreateRegex(@"m?(?<Name>qqbrowser|baiduboxapp|2345Explorer)[\/ ]?(?<Version>[\w\.]+)")  ,
                     // SouGouBrowser
                    Util.CreateRegex(@"(?<Name>metasr)[\/ ]?(?<Version>[\w\.]+)") ,
                },
                  Action = NameVersionAction
            },
            // LieBao Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // LieBao Browser
                    Util.CreateRegex(@"(?<Name>lbbrowser)")
                },
                Action = NameVersionAction
            },
            // Facebook App for iOS & Android
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"((?:fban\/fbios|fb_iab\/fb4a)(?!.+fbav)|;fbav\/(?<Version>[\w\.]+);)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Facebook";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Line App for iOS & Android /Chromium/Instagram
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Line App for iOS
                    Util.CreateRegex(@"safari (?<Name>line)\/(?<Version>[\w\.]+)")  ,
                    // Line App for Android
                    Util.CreateRegex(@"\b(?<Name>line)\/(?<Version>[\w\.]+)\/iab")  ,
                    // Chromium/Instagram
                    Util.CreateRegex(@"(?<Name>chromium|instagram)[\/ ](?<Version>[-\w\.]+)")
                },
                Action = NameVersionAction
            },
            // Google Search Appliance on iOS
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"\bgsa\/(?<Version>[\w\.]+) .*safari\/")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "GSA";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Chrome Headless
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"headlesschrome(?:\/(?<Version>[\w\.]+)| )")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Chrome Headless";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Chrome WebView
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"\swv\).+(chrome)\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Chrome WebView";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Android Browser
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"droid.+ version\/(?<Version>[\w\.]+)\b.+(?:mobile safari|safari)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Android Browser";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Chrome/OmniWeb/Arora/Tizen/Nokia
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"(?<Name>chrome|omniweb|arora|[tizenoka]{5} ?browser)\/v?(?<Version>[\w\.]+)"),
                },
                Action =NameVersionAction
            },
            // Mobile Safari
            new BrowserMatch{
                Regexes = new List<Regex>{
                   Util.CreateRegex(@"version\/(?<Version>[\w\.]+) .*mobile\/\w+ (safari)")
                 // Util.CreateRegex(@"version\/(?<Version>[\w\.]+) .*mobile\/\w+ (?<Name>safari)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Mobile Safari";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Safari & Safari Mobile
            new BrowserMatch{
                Regexes = new List<Regex>{
                    //Util.CreateRegex(@"version\/([\w\.]+) .*(mobile ?safari|safari)")
                    Util.CreateRegex(@"version\/(?<Version>[\w\.]+) .*(?<Name>mobile ?safari|safari)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = match.Groups["Name"].Value;
                    browser.Version =match.Groups["Version"].Value;
                }
            },
            // Safari < 3.0
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"webkit.+?(?<Name>mobile ?safari|safari)\/(?<Version>[\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = match.Groups["Name"].Value;

                   browser.Version= oldSafariMap.FirstOrDefault(
                       map=> match.Groups["Version"].Value.StartsWith(map.key)).value;

                     if (browser.Version is null)
                      browser.Version=match.Groups["Version"].Value;
                }
            },
            // I Don't Know
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"(webkit|khtml)\/(?<Version>[\w\.]+)")
                },
                Action = NameVersionAction
            },
            // Netscape
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Gecko based
                    Util.CreateRegex(@"(navigator|netscape\d?)\/(?<Version>[-\w\.]+)")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Netscape";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // Firefox Reality
            new BrowserMatch{
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"mobile vr; rv:(?<Version>[\w\.]+)\).+firefox")
                },
                Action = (Match match,Browser browser) =>{
                    browser.Name = "Firefox Reality";
                    browser.Version = match.Groups["Version"].Value;
                }
            },
            // All Others
            new BrowserMatch{
                Regexes = new List<Regex>{
                    // Flow
                    Util.CreateRegex(@"ekiohf.+(?<Name>flow)\/(?<Version>[\w\.]+)"),
                    // Swiftfox
                    Util.CreateRegex(@"(?<Name>swiftfox)"),
                    // IceDragon/Iceweasel/Camino/Chimera/Fennec/Maemo/Minimo/Conkeror/Klar
                    Util.CreateRegex(@"(?<Name>icedragon|iceweasel|camino|chimera|fennec|maemo browser|minimo|conkeror|klar)[\/ ]?(?<Version>[\w\.\+]+)"),
                    // Firefox/SeaMonkey/K-Meleon/IceCat/IceApe/Firebird/Phoenix
                    Util.CreateRegex(@"(?<Name>seamonkey|k-meleon|icecat|iceape|firebird|phoenix|palemoon|basilisk|waterfox)\/(?<Version>[-\w\.]+)$"),
                    // Other Firefox-based
                    Util.CreateRegex(@"(?<Name>firefox)\/(?<Version>[\w\.]+)"),
                    // Mozilla
                    Util.CreateRegex(@"(?<Name>mozilla)\/(?<Version>[\w\.]+) .+rv\:.+gecko\/\d+"),
                    // Polaris/Lynx/Dillo/iCab/Doris/Amaya/w3m/NetSurf/Sleipnir/Obigo/Mosaic/Go/ICE/UP.Browser
                    Util.CreateRegex(@"(?<Name>polaris|lynx|dillo|icab|doris|amaya|w3m|netsurf|sleipnir|obigo|mosaic|(?:go|ice|up)[\. ]?browser)[-\/ ]?v?(?<Version>[\w\.]+)"),
                    // Links
                    Util.CreateRegex(@"(?<Name>links) \((?<Version>[\w\.]+)"),
                },
                Action = NameVersionAction
            },
        };
    }
}