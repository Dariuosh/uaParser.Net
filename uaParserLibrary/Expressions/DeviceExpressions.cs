using System.Collections.Generic;
using System.Text.RegularExpressions;

using uaParserLibrary.Models;

using uaParserResource;

namespace uaParserLibrary.Expressions
{
    public static class DeviceExpressions
    {


        public static IReadOnlyList<DeviceMatch> Matches = new List<DeviceMatch>
        {
            //////////////////////////
            // MOBILES & TABLETS Ordered by popularity
            /////////////////////////

            // Samsung
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>sch-i[89]0\d|shw-m380s|sm-[pt]\w{2,4}|gt-[pn]\d{2,4}|sgh-t8[56]9|nexus 10)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =  Vendors.Samsung;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                      
                }
            },
            new DeviceMatch{
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>(?:s[cgp]h|gt|sm)-\w+|galaxy nexus)"),
                    Util.CreateRegex(@"samsung[- ](?<Model>[-\w]+)"),
                    Util.CreateRegex(@"sec-(?<Model>sgh\w+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Samsung;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                      
                }
            },
            // Apple
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // iPod/iPhone
                    Util.CreateRegex(@"\((?<Model>ip(?:hone|od)[\w ]*);"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor =  Vendors.Apple;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                      
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // iPad
                    Util.CreateRegex(@"\((?<Model>ipad);[-\w\),; ]+apple"),
                    Util.CreateRegex(@"applecoremedia\/[\w\.]+ \(?<Model>(ipad)"),
                    Util.CreateRegex(@"\b(?<Model>ipad)\d\d?,\d\d?[;\]].+ios"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor =  Vendors.Apple;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Huawei
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>(?:ag[rs][23]?|bah2?|sht?|btv)-a?[lw]\d{2})\b"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Huawei;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"(?:huawei|honor)(?<Model>[-\w ]+)[;\)]"),
                    Util.CreateRegex(@"\b(?<Model>nexus 6p|\w{2,4}-[atu]?[ln][01259x][012359][an]?)\b"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Huawei;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Xiaomi
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Xiaomi POCO
                    Util.CreateRegex(@"\b(?<Model>poco[\w ]+)(?: bui|\))"),
                // Xiaomi Hongmi 'numeric' models
                    Util.CreateRegex(@"\b; (?<Model>\w+) build\/hm\1"),
                // Xiaomi Hongmi
                    Util.CreateRegex(@"\b(?<Model>hm[-_ ]?note?[_ ]?(?:\d\w)?) bui"),
                // Xiaomi Redmi
                    Util.CreateRegex(@"\b(?<Model>redmi[\-_ ]?(?:note|k)?[\w_ ]+)(?: bui|\))"),
                // Xiaomi Mi
                    Util.CreateRegex(@"\b(?<Model>mi[-_ ]?(?:a\d|one|one[_ ]plus|note lte|max)?[_ ]?(?:\d?\w?)[_ ]?(?:plus|se|lite)?)(?: bui|\))"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Xiaomi;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Mi Pad tablets
                    Util.CreateRegex(@"\b(?<Model>mi[-_ ]?(?:pad)(?:[\w_ ]+))(?: bui|\))"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Xiaomi;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // OPPO
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"; (?<Model>\w+) bui.+ oppo"),
                    Util.CreateRegex(@"\b(?<Model>cph[12]\d{3}|p(?:af|c[al]|d\w|e[ar])[mt]\d0|x9007)\b"),
                },
                Action =   (Match match, Device device) =>
                {
                    device.Vendor = Vendors.OPPO;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Vivo
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"vivo (?<Model>\w+)(?: bui|\))"),
                    Util.CreateRegex(@"\b(?<Model>v[12]\d{3}\w?[at])(?: bui|;)"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Vivo;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Realme
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>rmx[12]\d{3})(?: bui|;|\))"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Realme;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Motorola
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>milestone|droid(?:[2-4x]| (?:bionic|x2|pro|razr))?:?( 4g)?)\b[\w ]+build\/"),
                    Util.CreateRegex(@"\bmot(?:orola)?[- ](?<Model>\w*)"),
                    Util.CreateRegex(@"(?<Model>(?:moto[\w\(\) ]+|xt\d{3,4}|nexus 6)(?= bui|\)))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Motorola;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>mz60\d|xoom[2 ]{0,2}) build\/"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Motorola;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // LG
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"(?<Model>(?=lg)?[vl]k\-?\d{3}) bui| 3\.[-\w; ]{10}lg?-([06cv9]{3,4})"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.LG;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"(?<Model>lm(?:-?f100[nv]?|-[\w\.]+)(?= bui|\))|nexus [45])"),
                    Util.CreateRegex(@"\blg[-e;\/ ]+(?<Model>(?!browser|netcast|android tv)\w+)"),
                    Util.CreateRegex(@"\blg-?(?<Model>[\d\w]+) bui"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.LG;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Lenovo
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(?<Model>ideatab[-\w ]+)"),
                    Util.CreateRegex(@"lenovo ?(?<Model>s[56]000[-\w]+|tab(?:[\w ]+)|yt[-\d\w]{6}|tb[-\d\w]{6})"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Lenovo;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Nokia
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(?:maemo|nokia).*(?<Model>n900|lumia \d+)"),
                    Util.CreateRegex(@"nokia[-_ ]?(?<Model>[-\w\.]*)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Nokia;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value.Replace("_"," ");
                    
                }
            },
            // Google
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // Google Pixel C
                    Util.CreateRegex(@"(?<Model>pixel c)\b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Google;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // Google Pixel
                    Util.CreateRegex(@"droid.+; (?<Model>pixel[\daxl ]{0,6})(?: bui|\))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Google;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Sony
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"droid.+ (?<Model>[c-g]\d{4}|so[-l]\w+|xq-a\w[4-7][12])(?= bui|\).+chrome\/(?![1-6]{0,1}\d\.))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Sony;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"sony tablet [ps]"),
                    Util.CreateRegex(@"\b(?:sony)?sgp\w+(?: bui|\))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Sony;
                    device.Type = DeviceType.Tablet;
                    device.Model ="Xperia Tablet";
                    
                }
            },
            // OnePlus
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {

                    Util.CreateRegex(@" (?<Model>kb2005|in20[12]5|be20[12][59])\b"),
                    Util.CreateRegex(@"(?:one)?(?:plus)? (?<Model>a\d0\d\d)(?: b|\))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.OnePlus;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Amazon
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Amazon
                    Util.CreateRegex(@"(?<Model>alexa)webm"),
                // Kindle Fire without Silk
                    Util.CreateRegex(@"(?<Model>kf[a-z]{2}wi)( bui|\))"),
                // Kindle Fire HD
                    Util.CreateRegex(@"(?<Model>kf[a-z]+)( bui|\)).+silk\/"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Amazon;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Fire Phone
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {

                    Util.CreateRegex(@"((?:sd|kf)[0349hijorstuw]+)( bui|\)).+silk\/"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Amazon;
                    device.Type = DeviceType.Mobile;
                    device.Model = "Fire Phone $1";
                    
                }
            },
            // BlackBerry
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // BlackBerry PlayBook
                    Util.CreateRegex(@"(?<Model>playbook);[-\w\),; ]+(rim)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.BlackBerry;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>(?:bb[a-f]|st[hv])100-\d)"),
                // BlackBerry 10
                    Util.CreateRegex(@"\(bb10; (?<Model>\w+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.BlackBerry;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Asus
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"(?:\b|asus_)(?<Model>transfo[prime ]{4,10} \w+|eeepc|slider \w+|nexus 7|padfone|p00[cj])"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.ASUS;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@" (?<Model>z[bes]6[027][012][km][ls]|zenfone \d\w?)\b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.ASUS;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // HTC 
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // HTC Nexus 9
                    Util.CreateRegex(@"(?<Model>nexus 9)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.HTC;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // HTC /ZTE /Alcatel/GeeksPhone/Nexian/Panasonic/Sony
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // HTC
                    Util.CreateRegex(@"(?<Vendor>htc)[-;_ ]{1,2}(?<Model>[\w ]+(?=\)| bui)|\w+)"),
                // ZTE
                    Util.CreateRegex(@"(?<Vendor>zte)[- ](?<Model>[\w ]+?)(?: bui|\/|\))"),
                // Alcatel/GeeksPhone/Nexian/Panasonic/Sony
                    Util.CreateRegex(@"(?<Vendor>alcatel|geeksphone|nexian|panasonic|sony)[-_ ]?(?<Model>[-\w]*)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value.Replace("_"," ");
                    
                }
            },
            // Acer
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"droid.+; (?<Model>[ab][1-7]-?[0178a]\d\d?)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.Acer;
                    device.Type = DeviceType.Tablet;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Meizu
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"droid.+; (?<Model>m[1-5] note) bui"),
                    Util.CreateRegex(@"\bmz-(?<Model>[-\w]{2,})"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.Meizu;
                    device.Type = DeviceType.Mobile;
                    device.Model =match.Groups[Keywords.Model].Value;
                    
                }
            },
            // MIXED  Mobile
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // MIXED
                    Util.CreateRegex(@"(?<Vendor>blackberry|benq|palm(?=\-)|sonyericsson|acer|asus|dell|meizu|motorola|polytron)[-_ ]?(?<Model>[-\w]*)"),
                // BlackBerry/BenQ/Palm/Sony-Ericsson/Acer/Asus/Dell/Meizu/Motorola/Polytron HP iPAQ
                    Util.CreateRegex(@"(?<Vendor>hp) (?<Model>[\w ]+\w)"),
                // Asus
                    Util.CreateRegex(@"(?<Vendor>asus)-?(?<Model>\w+)"),
                // Microsoft Lumia
                    Util.CreateRegex(@"(?<Vendor>microsoft); (?<Model>lumia[\w ]+)"),
                // Lenovo
                    Util.CreateRegex(@"(?<Vendor>lenovo)[-_ ]?(?<Model>[-\w]+)"),
                // Jolla
                    Util.CreateRegex(@"(?<Vendor>jolla)"),
                // OPPO
                    Util.CreateRegex(@"(?<Vendor>oppo) ?(?<Model>[\w ]+) bui"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            //MIXED Tablet
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // Archos
                    Util.CreateRegex(@"(?<Vendor>archos) (?<Model>gamepad2?)"),
                // HP TouchPad
                    Util.CreateRegex(@"(?<Vendor>hp).+(?<Model>touchpad(?!.+tablet)|tablet)"),
                // Kindle
                    Util.CreateRegex(@"(?<Vendor>kindle)\/(?<Model>[\w\.]+)"),
                // Nook
                    Util.CreateRegex(@"(?<Vendor>nook)[\w ]+build\/(?<Model>\w+)"),
                // Dell Streak
                    Util.CreateRegex(@"(?<Vendor>dell) (?<Model>strea[kpr\d ]*[\dko])"),
                // Le Pan Tablets
                    Util.CreateRegex(@"(?<Vendor>le[- ]+pan)[- ]+(?<Model>\w{1,9}) bui"),
                // Trinity Tablets
                    Util.CreateRegex(@"(?<Vendor>trinity)[- ]*(?<Model>t\d{3}) bui"),
                // Gigaset Tablets
                    Util.CreateRegex(@"(?<Vendor>gigaset)[- ]+(?<Model>q\w{1,9}) bui"),
                // Vodafone
                    Util.CreateRegex(@"(?<Vendor>vodafone) (?<Model>[\w ]+)(?:\)| bui)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Microsoft
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // Surface Duo
                    Util.CreateRegex(@"(?<Model>surface duo)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Microsoft;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Fairphone
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"droid [\d\.]+; (?<Model>fp\du?)(?: b|\))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Fairphone";
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // AT&T
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                // AT&T
                    Util.CreateRegex(@"(?<Model>u304aa)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.AT_T;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Siemens
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\bsie-(?<Model>\w*)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Siemens;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // RCA Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\b(?<Model>rct\w+) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "RCA";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Dell Venue Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\b(?<Model>venue[\d ]{2,7}) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Dell;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Verizon Tablet
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\b(?<Model>q(?:mv|ta)\w+) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Verizon;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Barnes & Noble Tablet
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\b(?:barnes[& ]+noble |bn[rt])(?<Model>[\w\+ ]*) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Barnes & Noble";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"\b(?<Model>tm\d{3}\w+) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "NuVision";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // ZTE
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // ZTE K Series Tablet
                    Util.CreateRegex(@"\b(?<Model>k88) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.ZTE;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // ZTE Nubia
                    Util.CreateRegex(@"\b(?<Model>nx\d{3}j) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.ZTE;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Swiss
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Swiss GEN Mobile
                    Util.CreateRegex(@"\b(?<Model>gen\d{3}) b.+49h"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Swiss;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Swiss ZUR Tablet
                    Util.CreateRegex(@"\b(?<Model>zur\d{3}) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Swiss;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Zeki Tablets
                    Util.CreateRegex(@"\b(?<Model>(zeki)?tb.*\b) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Zeki";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Dragon Touch Tablet
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(?<Model>[yr]\d{2}) b"),
                
                    Util.CreateRegex(@"\b(dragon[- ]+touch |dt)(?<Model>\w{5}) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Dragon Touch";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Insignia Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(?<Model>ns-?\w{0,9}) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Insignia";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // NextBook Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(?<Model>(nxa|next)-?\w{0,9}) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "NextBook";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Voice Xtreme Phones
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(xtreme_)?(?<Model>v(1[045]|2[015]|[3469]0|7[05])) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Voice";
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // LvTel Phones
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(lvtel\-)?(?<Model>v1[12]) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "LvTel";
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Essential PH-1
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(?<Model>ph-1) "),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Essential";
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Envizen Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(?<Model>v(100md|700na|7011|917g).*\b) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Envizen";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // MachSpeed Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                
                    Util.CreateRegex(@"\b(?<Model>trio[-\w\. ]+) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "MachSpeed";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Rotor Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\btu_(?<Model>1491) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Rotor";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Nvidia Shield Tablets
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"(?<Model>shield[\w ]+) b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = "Nvidia";
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Sprint Phones
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"(?<Vendor>sprint) (?<Model>\w+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Microsoft Kin
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"(?<Model>kin\.[onetw]{3})"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Microsoft;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value.Replace("."," ");
                    
                }
            },
            // Zebra
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"droid.+; (?<Model>cc6666?|et5[16]|mc[239][23]x?|vc8[03]x?)\)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Zebra;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"droid.+; (?<Model>ec30|ps20|tc[2-8]\d[kx])\)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Zebra;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },

            ///////////////////
            // CONSOLES
            ///////////////////

            // Ouya / Nintendo
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                // Ouya
                    Util.CreateRegex(@"(?<Vendor>ouya)"),
                // Nintendo
                    Util.CreateRegex(@"(?<Vendor>nintendo) (?<Model>[wids3utch]+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Console;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Nvidia
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"droid.+; (?<Model>shield) bui"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Nvidia;
                    device.Type = DeviceType.Console;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Playstation
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"(?<Model>playstation [345portablevi]+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Sony;
                    device.Type = DeviceType.Console;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Microsoft Xbox
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                
                    Util.CreateRegex(@"\b(?<Model>xbox(?: one)?(?!; xbox))[\); ]"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.Microsoft;
                    device.Type = DeviceType.Console;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            ///////////////////
            // SMARTTVS
            ///////////////////
                
            // Samsung
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"smart-tv.+(?<Vendor>samsung)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.SmartTV;
                    device.Model = Keywords.Undefined;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"hbbtv.+maple;(?<Model>\d+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Samsung;
                    device.Type = DeviceType.SmartTV;
                    device.Model =   string.Concat("SmartTV",match.Groups[Keywords.Model].Value);
                    
                }
            }, 
            // LG SmartTV
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(nux; netcast.+smarttv|lg (netcast\.tv-201\d|android tv))"),
                },
                Action =  (Match match, Device device) =>
                {
                    device.Vendor =Vendors.LG;
                    device.Type = DeviceType.SmartTV;
                    device.Model = Keywords.Undefined;
                    
                }
            },
            // Apple TV
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(apple) ?tv"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.SmartTV;
                    device.Model =   "Apple TV";
                    
                }
            },
            // Google Chromecast
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"crkey"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.Google;
                    device.Type = DeviceType.SmartTV;
                    device.Model = "Chromecast";
                    
                }
            },
            // Fire TV
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"droid.+aft(?<Model>\w)( bui|\))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Amazon;
                    device.Type = DeviceType.SmartTV;
                    device.Model = match.Groups[Keywords.Model].Value ;
                    
                }
            },
            // Sharp
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"\(dtv[\);].+(?<Model>aquos)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Sharp;
                    device.Type = DeviceType.SmartTV;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // HbbTV devices /Roku
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                // Roku
                    Util.CreateRegex(@"\b(?<Vendor>roku)\s*[\dx]*[\)\/](?<Model>(?:dvp-)?[\d\.]*)"),
                // HbbTV devices
                    Util.CreateRegex(@"hbbtv\/\d+\.\d+\.\d+ +\([\w ]*; *(?<Vendor>\w[^;]*);(?<Model>[^;]*)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value.Trim();
                    device.Type = DeviceType.SmartTV;
                    device.Model = match.Groups[Keywords.Model].Value.Trim();
                    
                }
            }, 
            // SmartTV from Unidentified Vendors
            new DeviceMatch
            {
                Regexes = new List<Regex>
                {
                    Util.CreateRegex(@"\b(android tv|smart[- ]?tv|opera tv|tv; rv:)\b"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Undefined;
                    device.Type = DeviceType.SmartTV;
                    device.Model = Keywords.Undefined;
                    
                }
            },
            ///////////////////
            // WEARABLES
            ///////////////////

            // Pebble
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"((?<Vendor>pebble))app"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Wearable;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            }, 
            // Google Glass
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"droid.+; (?<Model>glass) \d"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor =Vendors.Google;
                    device.Type = DeviceType.Wearable;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            new DeviceMatch
            {
                Regexes = new List<Regex>{
                    Util.CreateRegex(@"droid.+; (?<Model>wt63?0{2,3})\)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Vendors.Zebra;
                    device.Type = DeviceType.Wearable;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },

            ///////////////////
            // EMBEDDED
            ///////////////////

            // Tesla
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(?<Vendor>tesla)(?: qtcarbrowser|\/[-\w\.]+)"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = match.Groups[Keywords.Vendor].Value;
                    device.Type = DeviceType.Embedded;
                    device.Model = Keywords.Undefined ;
                    
                }
            },

            ////////////////////
            // MIXED (GENERIC)
            ///////////////////

            // Android Phones from Unidentified Vendors
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"droid .+?; (?<Model>[^;]+?)(?: bui|\) applew).+? mobile safari"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Undefined;
                    device.Type = DeviceType.Mobile;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Android Tablets from Unidentified Vendors
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"droid .+?; (?<Model>[^;]+?)(?: bui|\) applew).+?(?! mobile) safari"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Undefined;
                    device.Type = DeviceType.Tablet;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            },
            // Unidentifiable Tablet
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"\b((tablet|tab)[;\/]|focus\/\d(?!.+mobile))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Undefined;
                    device.Type = DeviceType.Tablet;
                    device.Model = Keywords.Undefined;
                    
                }
            },
            // Unidentifiable Mobile
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(phone|mobile(?:[;\/]| safari)|pda(?=.+windows ce))"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Undefined;
                    device.Type = DeviceType.Mobile;
                    device.Model = Keywords.Undefined;
                    
                }
            },
            // Generic Android Device
            new DeviceMatch
            {
                Regexes = new List<Regex>{

                    Util.CreateRegex(@"(?<Model>android[-\w\. ]{0,9});.+buil"),
                },
                Action = (Match match, Device device) =>
                {
                    device.Vendor = Keywords.Generic;
                    device.Type = Keywords.Undefined;
                    device.Model = match.Groups[Keywords.Model].Value;
                    
                }
            }
        };
    }
}