[[

//////////////////////////
// MOBILES & TABLETS
// Ordered by popularity
/////////////////////////

// Samsung
/\b(sch-i[89]0\d|shw-m380s|sm-[pt]\w{2,4}|gt-[pn]\d{2,4}|sgh-t8[56]9|nexus 10)/i
], [MODEL, [VENDOR, SAMSUNG], [TYPE, TABLET]], [
    /\b((?:s[cgp]h|gt|sm)-\w+|galaxy nexus)/i,
    /samsung[- ]([-\w]+)/i,
    /sec-(sgh\w+)/i
], [MODEL, [VENDOR, SAMSUNG], [TYPE, MOBILE]], [

        // Apple
        /\((ip(?:hone|od)[\w ]*);/i                                         // iPod/iPhone
    ], [MODEL, [VENDOR, APPLE], [TYPE, MOBILE]], [
        /\((ipad);[-\w\),; ]+apple/i,                                       // iPad
        /applecoremedia\/[\w\.]+ \((ipad)/i,
        /\b(ipad)\d\d?,\d\d?[;\]].+ios/i
    ], [MODEL, [VENDOR, APPLE], [TYPE, TABLET]], [

        // Huawei
        /\b((?:ag[rs][23]?|bah2?|sht?|btv)-a?[lw]\d{2})\b/i
    ], [MODEL, [VENDOR, HUAWEI], [TYPE, TABLET]], [
        /(?:huawei|honor)([-\w ]+)[;\)]/i,
        /\b(nexus 6p|\w{2,4}-[atu]?[ln][01259x][012359][an]?)\b/i
    ], [MODEL, [VENDOR, HUAWEI], [TYPE, MOBILE]], [

        // Xiaomi
        /\b(poco[\w ]+)(?: bui|\))/i,                                       // Xiaomi POCO
        /\b; (\w+) build\/hm\1/i,                                           // Xiaomi Hongmi 'numeric' models
        /\b(hm[-_ ]?note?[_ ]?(?:\d\w)?) bui/i,                             // Xiaomi Hongmi
        /\b(redmi[\-_ ]?(?:note|k)?[\w_ ]+)(?: bui|\))/i,                   // Xiaomi Redmi
        /\b(mi[-_ ]?(?:a\d|one|one[_ ]plus|note lte|max)?[_ ]?(?:\d?\w?)[_ ]?(?:plus|se|lite)?)(?: bui|\))/i // Xiaomi Mi
    ], [[MODEL, /_/g, ' '], [VENDOR, XIAOMI], [TYPE, MOBILE]], [
        /\b(mi[-_ ]?(?:pad)(?:[\w_ ]+))(?: bui|\))/i                        // Mi Pad tablets
    ], [[MODEL, /_/g, ' '], [VENDOR, XIAOMI], [TYPE, TABLET]], [

        // OPPO
        /; (\w+) bui.+ oppo/i,
        /\b(cph[12]\d{3}|p(?:af|c[al]|d\w|e[ar])[mt]\d0|x9007)\b/i
    ], [MODEL, [VENDOR, 'OPPO'], [TYPE, MOBILE]], [

        // Vivo
        /vivo (\w+)(?: bui|\))/i,
        /\b(v[12]\d{3}\w?[at])(?: bui|;)/i
    ], [MODEL, [VENDOR, 'Vivo'], [TYPE, MOBILE]], [

        // Realme
        /\b(rmx[12]\d{3})(?: bui|;|\))/i
    ], [MODEL, [VENDOR, 'Realme'], [TYPE, MOBILE]], [

        // Motorola
        /\b(milestone|droid(?:[2-4x]| (?:bionic|x2|pro|razr))?:?( 4g)?)\b[\w ]+build\//i,
        /\bmot(?:orola)?[- ](\w*)/i,
        /((?:moto[\w\(\) ]+|xt\d{3,4}|nexus 6)(?= bui|\)))/i
    ], [MODEL, [VENDOR, MOTOROLA], [TYPE, MOBILE]], [
        /\b(mz60\d|xoom[2 ]{0,2}) build\//i
    ], [MODEL, [VENDOR, MOTOROLA], [TYPE, TABLET]], [

        // LG
        /((?=lg)?[vl]k\-?\d{3}) bui| 3\.[-\w; ]{10}lg?-([06cv9]{3,4})/i
    ], [MODEL, [VENDOR, LG], [TYPE, TABLET]], [
        /(lm(?:-?f100[nv]?|-[\w\.]+)(?= bui|\))|nexus [45])/i,
        /\blg[-e;\/ ]+((?!browser|netcast|android tv)\w+)/i,
        /\blg-?([\d\w]+) bui/i
    ], [MODEL, [VENDOR, LG], [TYPE, MOBILE]], [

        // Lenovo
        /(ideatab[-\w ]+)/i,
        /lenovo ?(s[56]000[-\w]+|tab(?:[\w ]+)|yt[-\d\w]{6}|tb[-\d\w]{6})/i
    ], [MODEL, [VENDOR, 'Lenovo'], [TYPE, TABLET]], [

        // Nokia
        /(?:maemo|nokia).*(n900|lumia \d+)/i,
        /nokia[-_ ]?([-\w\.]*)/i
    ], [[MODEL, /_/g, ' '], [VENDOR, 'Nokia'], [TYPE, MOBILE]], [

        // Google
        /(pixel c)\b/i                                                      // Google Pixel C
    ], [MODEL, [VENDOR, GOOGLE], [TYPE, TABLET]], [
        /droid.+; (pixel[\daxl ]{0,6})(?: bui|\))/i                         // Google Pixel
    ], [MODEL, [VENDOR, GOOGLE], [TYPE, MOBILE]], [

        // Sony
        /droid.+ ([c-g]\d{4}|so[-l]\w+|xq-a\w[4-7][12])(?= bui|\).+chrome\/(?![1-6]{0,1}\d\.))/i
    ], [MODEL, [VENDOR, SONY], [TYPE, MOBILE]], [
        /sony tablet [ps]/i,
        /\b(?:sony)?sgp\w+(?: bui|\))/i
    ], [[MODEL, 'Xperia Tablet'], [VENDOR, SONY], [TYPE, TABLET]], [

        // OnePlus
        / (kb2005|in20[12]5|be20[12][59])\b/i,
        /(?:one)?(?:plus)? (a\d0\d\d)(?: b|\))/i
    ], [MODEL, [VENDOR, 'OnePlus'], [TYPE, MOBILE]], [

        // Amazon
        /(alexa)webm/i,
        /(kf[a-z]{2}wi)( bui|\))/i,                                         // Kindle Fire without Silk
        /(kf[a-z]+)( bui|\)).+silk\//i                                      // Kindle Fire HD
    ], [MODEL, [VENDOR, AMAZON], [TYPE, TABLET]], [
        /((?:sd|kf)[0349hijorstuw]+)( bui|\)).+silk\//i                     // Fire Phone
    ], [[MODEL, /(.+)/g, 'Fire Phone $1'], [VENDOR, AMAZON], [TYPE, MOBILE]], [

        // BlackBerry
        /(playbook);[-\w\),; ]+(rim)/i                                      // BlackBerry PlayBook
    ], [MODEL, VENDOR, [TYPE, TABLET]], [
        /\b((?:bb[a-f]|st[hv])100-\d)/i,
        /\(bb10; (\w+)/i                                                    // BlackBerry 10
    ], [MODEL, [VENDOR, BLACKBERRY], [TYPE, MOBILE]], [

        // Asus
        /(?:\b|asus_)(transfo[prime ]{4,10} \w+|eeepc|slider \w+|nexus 7|padfone|p00[cj])/i
    ], [MODEL, [VENDOR, ASUS], [TYPE, TABLET]], [
        / (z[bes]6[027][012][km][ls]|zenfone \d\w?)\b/i
    ], [MODEL, [VENDOR, ASUS], [TYPE, MOBILE]], [

        // HTC
        /(nexus 9)/i                                                        // HTC Nexus 9
    ], [MODEL, [VENDOR, 'HTC'], [TYPE, TABLET]], [
        /(htc)[-;_ ]{1,2}([\w ]+(?=\)| bui)|\w+)/i,                         // HTC

        // ZTE
        /(zte)[- ]([\w ]+?)(?: bui|\/|\))/i,
        /(alcatel|geeksphone|nexian|panasonic|sony)[-_ ]?([-\w]*)/i         // Alcatel/GeeksPhone/Nexian/Panasonic/Sony
    ], [VENDOR, [MODEL, /_/g, ' '], [TYPE, MOBILE]], [

        // Acer
        /droid.+; ([ab][1-7]-?[0178a]\d\d?)/i
    ], [MODEL, [VENDOR, 'Acer'], [TYPE, TABLET]], [

        // Meizu
        /droid.+; (m[1-5] note) bui/i,
        /\bmz-([-\w]{2,})/i
    ], [MODEL, [VENDOR, 'Meizu'], [TYPE, MOBILE]], [

        // MIXED
        /(blackberry|benq|palm(?=\-)|sonyericsson|acer|asus|dell|meizu|motorola|polytron)[-_ ]?([-\w]*)/i,
        // BlackBerry/BenQ/Palm/Sony-Ericsson/Acer/Asus/Dell/Meizu/Motorola/Polytron
        /(hp) ([\w ]+\w)/i,                                                 // HP iPAQ
        /(asus)-?(\w+)/i,                                                   // Asus
        /(microsoft); (lumia[\w ]+)/i,                                      // Microsoft Lumia
        /(lenovo)[-_ ]?([-\w]+)/i,                                          // Lenovo
        /(jolla)/i,                                                         // Jolla
        /(oppo) ?([\w ]+) bui/i                                             // OPPO
    ], [VENDOR, MODEL, [TYPE, MOBILE]], [

        /(archos) (gamepad2?)/i,                                            // Archos
        /(hp).+(touchpad(?!.+tablet)|tablet)/i,                             // HP TouchPad
        /(kindle)\/([\w\.]+)/i,                                             // Kindle
        /(nook)[\w ]+build\/(\w+)/i,                                        // Nook
        /(dell) (strea[kpr\d ]*[\dko])/i,                                   // Dell Streak
        /(le[- ]+pan)[- ]+(\w{1,9}) bui/i,                                  // Le Pan Tablets
        /(trinity)[- ]*(t\d{3}) bui/i,                                      // Trinity Tablets
        /(gigaset)[- ]+(q\w{1,9}) bui/i,                                    // Gigaset Tablets
        /(vodafone) ([\w ]+)(?:\)| bui)/i                                   // Vodafone
    ], [VENDOR, MODEL, [TYPE, TABLET]], [

        /(surface duo)/i                                                    // Surface Duo
    ], [MODEL, [VENDOR, MICROSOFT], [TYPE, TABLET]], [
        /droid [\d\.]+; (fp\du?)(?: b|\))/i                                 // Fairphone
    ], [MODEL, [VENDOR, 'Fairphone'], [TYPE, MOBILE]], [
        /(u304aa)/i                                                         // AT&T
    ], [MODEL, [VENDOR, 'AT&T'], [TYPE, MOBILE]], [
        /\bsie-(\w*)/i                                                      // Siemens
    ], [MODEL, [VENDOR, 'Siemens'], [TYPE, MOBILE]], [
        /\b(rct\w+) b/i                                                     // RCA Tablets
    ], [MODEL, [VENDOR, 'RCA'], [TYPE, TABLET]], [
        /\b(venue[\d ]{2,7}) b/i                                            // Dell Venue Tablets
    ], [MODEL, [VENDOR, 'Dell'], [TYPE, TABLET]], [
        /\b(q(?:mv|ta)\w+) b/i                                              // Verizon Tablet
    ], [MODEL, [VENDOR, 'Verizon'], [TYPE, TABLET]], [
        /\b(?:barnes[& ]+noble |bn[rt])([\w\+ ]*) b/i                       // Barnes & Noble Tablet
    ], [MODEL, [VENDOR, 'Barnes & Noble'], [TYPE, TABLET]], [
        /\b(tm\d{3}\w+) b/i
    ], [MODEL, [VENDOR, 'NuVision'], [TYPE, TABLET]], [
        /\b(k88) b/i                                                        // ZTE K Series Tablet
    ], [MODEL, [VENDOR, 'ZTE'], [TYPE, TABLET]], [
        /\b(nx\d{3}j) b/i                                                   // ZTE Nubia
    ], [MODEL, [VENDOR, 'ZTE'], [TYPE, MOBILE]], [
        /\b(gen\d{3}) b.+49h/i                                              // Swiss GEN Mobile
    ], [MODEL, [VENDOR, 'Swiss'], [TYPE, MOBILE]], [
        /\b(zur\d{3}) b/i                                                   // Swiss ZUR Tablet
    ], [MODEL, [VENDOR, 'Swiss'], [TYPE, TABLET]], [
        /\b((zeki)?tb.*\b) b/i                                              // Zeki Tablets
    ], [MODEL, [VENDOR, 'Zeki'], [TYPE, TABLET]], [
        /\b([yr]\d{2}) b/i,
        /\b(dragon[- ]+touch |dt)(\w{5}) b/i                                // Dragon Touch Tablet
    ], [[VENDOR, 'Dragon Touch'], MODEL, [TYPE, TABLET]], [
        /\b(ns-?\w{0,9}) b/i                                                // Insignia Tablets
    ], [MODEL, [VENDOR, 'Insignia'], [TYPE, TABLET]], [
        /\b((nxa|next)-?\w{0,9}) b/i                                        // NextBook Tablets
    ], [MODEL, [VENDOR, 'NextBook'], [TYPE, TABLET]], [
        /\b(xtreme\_)?(v(1[045]|2[015]|[3469]0|7[05])) b/i                  // Voice Xtreme Phones
    ], [[VENDOR, 'Voice'], MODEL, [TYPE, MOBILE]], [
        /\b(lvtel\-)?(v1[12]) b/i                                           // LvTel Phones
    ], [[VENDOR, 'LvTel'], MODEL, [TYPE, MOBILE]], [
        /\b(ph-1) /i                                                        // Essential PH-1
    ], [MODEL, [VENDOR, 'Essential'], [TYPE, MOBILE]], [
        /\b(v(100md|700na|7011|917g).*\b) b/i                               // Envizen Tablets
    ], [MODEL, [VENDOR, 'Envizen'], [TYPE, TABLET]], [
        /\b(trio[-\w\. ]+) b/i                                              // MachSpeed Tablets
    ], [MODEL, [VENDOR, 'MachSpeed'], [TYPE, TABLET]], [
        /\btu_(1491) b/i                                                    // Rotor Tablets
    ], [MODEL, [VENDOR, 'Rotor'], [TYPE, TABLET]], [
        /(shield[\w ]+) b/i                                                 // Nvidia Shield Tablets
    ], [MODEL, [VENDOR, 'Nvidia'], [TYPE, TABLET]], [
        /(sprint) (\w+)/i                                                   // Sprint Phones
    ], [VENDOR, MODEL, [TYPE, MOBILE]], [
        /(kin\.[onetw]{3})/i                                                // Microsoft Kin
    ], [[MODEL, /\./g, ' '], [VENDOR, MICROSOFT], [TYPE, MOBILE]], [
        /droid.+; (cc6666?|et5[16]|mc[239][23]x?|vc8[03]x?)\)/i             // Zebra
    ], [MODEL, [VENDOR, ZEBRA], [TYPE, TABLET]], [
        /droid.+; (ec30|ps20|tc[2-8]\d[kx])\)/i
    ], [MODEL, [VENDOR, ZEBRA], [TYPE, MOBILE]], [

        ///////////////////
        // CONSOLES
        ///////////////////

        /(ouya)/i,                                                          // Ouya
        /(nintendo) ([wids3utch]+)/i                                        // Nintendo
    ], [VENDOR, MODEL, [TYPE, CONSOLE]], [
        /droid.+; (shield) bui/i                                            // Nvidia
    ], [MODEL, [VENDOR, 'Nvidia'], [TYPE, CONSOLE]], [
        /(playstation [345portablevi]+)/i                                   // Playstation
    ], [MODEL, [VENDOR, SONY], [TYPE, CONSOLE]], [
        /\b(xbox(?: one)?(?!; xbox))[\); ]/i                                // Microsoft Xbox
    ], [MODEL, [VENDOR, MICROSOFT], [TYPE, CONSOLE]], [

        ///////////////////
        // SMARTTVS
        ///////////////////

        /smart-tv.+(samsung)/i                                              // Samsung
    ], [VENDOR, [TYPE, SMARTTV]], [
        /hbbtv.+maple;(\d+)/i
    ], [[MODEL, /^/, 'SmartTV'], [VENDOR, SAMSUNG], [TYPE, SMARTTV]], [
        /(nux; netcast.+smarttv|lg (netcast\.tv-201\d|android tv))/i        // LG SmartTV
    ], [[VENDOR, LG], [TYPE, SMARTTV]], [
        /(apple) ?tv/i                                                      // Apple TV
    ], [VENDOR, [MODEL, APPLE + ' TV'], [TYPE, SMARTTV]], [
        /crkey/i                                                            // Google Chromecast
    ], [[MODEL, CHROME + 'cast'], [VENDOR, GOOGLE], [TYPE, SMARTTV]], [
        /droid.+aft(\w)( bui|\))/i                                          // Fire TV
    ], [MODEL, [VENDOR, AMAZON], [TYPE, SMARTTV]], [
        /\(dtv[\);].+(aquos)/i                                              // Sharp
    ], [MODEL, [VENDOR, 'Sharp'], [TYPE, SMARTTV]], [
        /\b(roku)[\dx]*[\)\/]((?:dvp-)?[\d\.]*)/i,                          // Roku
        /hbbtv\/\d+\.\d+\.\d+ +\([\w ]*; *(\w[^;]*);([^;]*)/i               // HbbTV devices
    ], [[VENDOR, trim], [MODEL, trim], [TYPE, SMARTTV]], [
        /\b(android tv|smart[- ]?tv|opera tv|tv; rv:)\b/i                   // SmartTV from Unidentified Vendors
    ], [[TYPE, SMARTTV]], [

        ///////////////////
        // WEARABLES
        ///////////////////

        /((pebble))app/i                                                    // Pebble
    ], [VENDOR, MODEL, [TYPE, WEARABLE]], [
        /droid.+; (glass) \d/i                                              // Google Glass
    ], [MODEL, [VENDOR, GOOGLE], [TYPE, WEARABLE]], [
        /droid.+; (wt63?0{2,3})\)/i
    ], [MODEL, [VENDOR, ZEBRA], [TYPE, WEARABLE]], [

        ///////////////////
        // EMBEDDED
        ///////////////////

        /(tesla)(?: qtcarbrowser|\/[-\w\.]+)/i                              // Tesla
    ], [VENDOR, [TYPE, EMBEDDED]], [

        ////////////////////
        // MIXED (GENERIC)
        ///////////////////

        /droid .+?; ([^;]+?)(?: bui|\) applew).+? mobile safari/i           // Android Phones from Unidentified Vendors
    ], [MODEL, [TYPE, MOBILE]], [
        /droid .+?; ([^;]+?)(?: bui|\) applew).+?(?! mobile) safari/i       // Android Tablets from Unidentified Vendors
    ], [MODEL, [TYPE, TABLET]], [
        /\b((tablet|tab)[;\/]|focus\/\d(?!.+mobile))/i                      // Unidentifiable Tablet
    ], [[TYPE, TABLET]], [
        /(phone|mobile(?:[;\/]| safari)|pda(?=.+windows ce))/i              // Unidentifiable Mobile
    ], [[TYPE, MOBILE]], [
        /(android[-\w\. ]{0,9});.+buil/i                                    // Generic Android Device
    ], [MODEL, [VENDOR, 'Generic']]
]