'use strict';

// uaParser.Net's bot rules: which user agents belong to crawlers, link previewers, monitors,
// scanners, HTTP libraries and other software that is not a person using a browser.
//
// The list of bots is crawler-user-agents.json, an unchanged copy of
// https://github.com/monperrus/crawler-user-agents (commit below), MIT License,
// Copyright (c) 2017 Martin Monperrus. This file only adds uaParser.Net's changes to it
// (each with its reason), a few bots the list does not have, and a fallback for unknown bots.
//
// detect(ua) is the reference: tools/RuleGenerator writes what it gives for every test user
// agent, and the C# detector (uaParserLibrary/Parsing/BotDetector.cs) must give the same.
//
// How a bot is chosen when several patterns match: the match that starts first in the user
// agent wins, then the longest match, then the pattern that comes first in the list. (On the
// list's own examples this picks the example's own entry more often than list order alone.)

const crawlers = require('./crawler-user-agents.json');

const SOURCE = {
    name: 'crawler-user-agents',
    url: 'https://github.com/monperrus/crawler-user-agents',
    commit: '9345a7ad9c49cd0fbd880eb5a84ed1f358fc6a97',
};

// Patterns of the list that uaParser.Net changes: pattern -> new pattern, or null to drop it.
const REPLACE = {
    // "Butterfly" also matches the HTC Butterfly phone; the bot always writes "Butterfly/".
    'Butterfly': 'Butterfly\\/',
    // "Sonic" also matches phones such as the "Lucky Ultra Sonic".
    'Sonic': '(?:RankSonicSiteAuditor|\\bSonic)\\/',
    // "speedy" also matches Yahoo Mail on HTC phones ("speedy;HTC"); the bot is "Speedy Spider".
    'speedy': 'Speedy ?Spider',
    // "008/" also matches feature phones ("sprd-L008/1.0"); the bot's user agent starts with it.
    '008\\/': '^008\\/',
    // An Android build number (OPPO phones with Android 15 use it), not a bot.
    'AP3A\\.240617\\.008': null,
};

// Bots the list does not have. Same fields as the list.
const EXTRA = [
    {
        pattern: '^Java\\/',
        description: 'The HTTP client of the Java runtime (java.net.HttpURLConnection)',
        tags: ['http-library'],
        instances: ['Java/17.0.12', 'Java/1.8.0_412'],
    },
    {
        pattern: '^PostmanRuntime\\/',
        url: 'https://www.postman.com/',
        description: 'Postman, a tool for testing web APIs',
        tags: ['http-library'],
        instances: ['PostmanRuntime/7.39.0'],
    },
    {
        pattern: '^insomnia\\/',
        url: 'https://insomnia.rest/',
        description: 'Insomnia, a tool for testing web APIs',
        tags: ['http-library'],
        instances: ['insomnia/2023.5.8'],
    },
];

// Names for patterns that are web addresses or are otherwise not a readable name. Every other
// name is the pattern's text (for a pattern that is plain text) or the text it matches in the
// list's first example.
const NAMES = {
    'Code\\/1\\.': 'Visual Studio Code',
    'developers\\.google\\.com\\/\\+\\/web\\/snippet': 'Google Web Snippet',
    'filterdb\\.iss\\.net\\/crawler': 'oBot',
    'https:\\/\\/developers\\.cloudflare\\.com\\/security-center': 'Cloudflare Security Center',
    'loc\\.gov\\/programs\\/web-archiving': 'Library of Congress',
    'pinterest\\.com\\/bot': 'Pinterest',
    'sindresorhus\\/got': 'got',
    'yandex\\.com\\/bots': 'Yandex',
    // Patterns that are not plain text.
    'AdsBot-Google([^-]|$)': 'AdsBot-Google',
    '[wW]get': 'Wget',
    '(sistrix|SISTRIX) [cC]rawler': 'SISTRIX Crawler',
    'Ahrefs(Bot|SiteAudit)': 'Ahrefs',
    'S[eE][mM]rushBot': 'SemrushBot',
    'Livelap[bB]ot': 'LivelapBot',
    '[pP]ingdom': 'Pingdom',
    'Bark[rR]owler': 'Barkrowler',
    '(^| )sentry\\/': 'Sentry',
    '[Cc]urebot': 'Curebot',
    'BlogTraffic\\/\\d\\.\\d+ Feed-Fetcher': 'BlogTraffic Feed-Fetcher',
    '(^| )PTST\\/': 'WebPageTest',
    '[cC]laude[bB]ot': 'ClaudeBot',
    '[aA]cunetix': 'Acunetix',
    '[dD]ir[Bb]uster': 'DirBuster',
    '[mM]echanize': 'Mechanize',
    'Automaton|Newsify Feed Fetcher': 'Newsify',
    'Chirp|gotosocial': 'GoToSocial',
    '[cC]ludo': 'Cludo',
    'ContextualBot[\\s\\S]*outcomes\\.net': 'ContextualBot',
    'Current[\\s\\S]*RSS Reader': 'Current RSS Reader',
    'Netumo|netumo': 'Netumo',
    'Spider[\\s\\S]*spider\\.com': 'Spider',
    'Unshorten\\.It\\!': 'Unshorten.It!',
    // uaParser.Net's own patterns (REPLACE).
    '(?:RankSonicSiteAuditor|\\bSonic)\\/': 'Sonic',
    'Speedy ?Spider': 'Speedy Spider',
};

// Unknown bots: a word that ends in bot, crawler, spider or scraper, followed by "/", ";", ")",
// a space or the end. Cubot is a phone maker. The name is the word.
const FALLBACK = /\b(?!cubot\b)([\w-]*(?:bot|crawler|spider|scraper))\b(?=[\/;) ]|$)/i;

// A pattern that is plain text: letters, digits, some punctuation and escaped punctuation, with
// an optional ^ at the start and $ at the end. These are found with a multi-string search
// instead of a regex.
const PLAIN = /^\^?(?:[A-Za-z0-9 _\-;:,!@%&=~'"<>]|\\[\/.()\-+\[\]?*|{}])+\$?$/;

// The text a plain pattern matches.
function literal(pattern) {
    return pattern.replace(/^\^/, '').replace(/\$$/, '').replace(/\\(.)/g, '$1');
}

function nameOf(pattern, instances) {
    if (NAMES[pattern]) return NAMES[pattern];
    if (!PLAIN.test(pattern)) {
        throw new Error(`bot-rules.js: add a name to NAMES for the pattern /${pattern}/`);
    }
    let name = literal(pattern).replace(/[\/;:,\s(.\-]+$/, '').replace(/^[;\s]+/, '');
    // "Foo)" from "(compatible; Foo)" and "Foo (" from "Foo (+http...".
    if (name.endsWith(')') && !name.includes('(')) name = name.slice(0, -1);
    const open = name.indexOf(' (');
    if (open > 0 && !name.slice(open).includes(')')) name = name.slice(0, open);
    if (!name) throw new Error(`bot-rules.js: no name for the pattern /${pattern}/ (example: ${instances[0]})`);
    return name;
}

// The bots in priority order (the list, then EXTRA), each with { pattern, name, category, url,
// instances, regex }. category is the list's first tag.
function bots() {
    const unknown = Object.keys(REPLACE).filter(p => !crawlers.some(c => c.pattern === p));
    if (unknown.length) throw new Error(`bot-rules.js: REPLACE names patterns the list does not have: ${unknown.join(', ')}`);

    const list = [];
    for (const entry of [...crawlers, ...EXTRA]) {
        const replaced = Object.prototype.hasOwnProperty.call(REPLACE, entry.pattern);
        const pattern = replaced ? REPLACE[entry.pattern] : entry.pattern;
        if (pattern === null) continue;
        const instances = entry.instances || [];
        list.push({
            pattern,
            name: nameOf(pattern, instances),
            category: (entry.tags || [])[0] || null,
            url: entry.url || null,
            instances,
            regex: new RegExp(pattern),
        });
    }
    return list;
}

// The list's examples, and EXTRA's, of the bots uaParser.Net keeps.
function examples() {
    return bots().flatMap(bot => bot.instances);
}

let cached;

// The bot a user agent belongs to: { name, category, url }, or null for none. text is the user
// agent as the browser rules see it (see parse in user-agent-rules.js: at most 500 characters).
function detect(text) {
    cached = cached || bots();
    let best = null;
    for (let i = 0; i < cached.length; i++) {
        const m = cached[i].regex.exec(text);
        if (!m) continue;
        if (!best || m.index < best.index || (m.index === best.index && m[0].length > best.length)) {
            best = { index: m.index, length: m[0].length, bot: cached[i] };
        }
    }
    if (best) return { name: best.bot.name, category: best.bot.category, url: best.bot.url };
    const m = FALLBACK.exec(text);
    return m ? { name: m[1], category: null, url: null } : null;
}

module.exports = { SOURCE, REPLACE, EXTRA, FALLBACK, PLAIN, literal, bots, examples, detect };
