'use strict';

// Turns uaParser.Net's rule table (rules/user-agent-rules.js) into C#
// (uaParserLibrary/Rules/UserAgentRules.g.cs).
//
// Each rule is [regexes, properties]. rgxMapper tries the regexes in order; for the
// first that matches, property p is set from capture group p + 1. A property is either a
// plain name (copy the group) or an array whose shape decides what is stored. Every shape is
// mapped to an Assignment factory in uaParserLibrary/Parsing/Assignment.cs. Anything the
// generator does not recognise stops it, so nothing is silently converted wrong.

const { requiredWords } = require('./prefilter');

const CATEGORIES = { browser: 'Browser', cpu: 'Cpu', device: 'Device', engine: 'Engine', os: 'Os' };
const FIELDS = {
    name: 'Name', version: 'Version', major: 'Major', architecture: 'Architecture',
    vendor: 'Vendor', model: 'Model', type: 'Type',
};

function fail(where, message) {
    throw new Error(`${where}: ${message}`);
}

// C# regular string literal; non-ASCII characters are escaped so the file stays ASCII.
function csString(value) {
    if (value === undefined || value === null) return 'null';
    if (typeof value !== 'string') fail('string', `expected a string, got ${typeof value}`);
    let out = '"';
    for (const ch of value) {
        const code = ch.codePointAt(0);
        if (ch === '"' || ch === '\\') out += '\\' + ch;
        else if (code < 0x20 || code > 0x7e) {
            out += code > 0xffff
                ? ch.split('').map(u => '\\u' + u.charCodeAt(0).toString(16).padStart(4, '0')).join('')
                : '\\u' + code.toString(16).padStart(4, '0');
        } else out += ch;
    }
    return out + '"';
}

// C# verbatim string for a regex pattern (only '"' needs doubling).
function csPattern(source) {
    return '@"' + source.replace(/"/g, '""') + '"';
}

// .NET reads "[a-[b]]" as character class subtraction; JavaScript does not.
function checkPattern(where, source) {
    let inClass = false;
    for (let i = 0; i < source.length; i++) {
        const ch = source[i];
        if (ch === '\\') { i++; continue; }
        if (!inClass && ch === '[') inClass = true;
        else if (inClass && ch === ']') inClass = false;
        else if (inClass && ch === '-' && source[i + 1] === '[') fail(where, `"-[" inside a character class: ${source}`);
    }
}

// JavaScript replacement string -> .NET replacement string.
function csReplacement(where, js) {
    if (typeof js !== 'string') fail(where, 'replacement is not a string');
    let out = '';
    for (let i = 0; i < js.length; i++) {
        const ch = js[i];
        if (ch !== '$') { out += ch; continue; }
        const next = js[i + 1];
        if (next === '$') { out += '$$'; i++; }
        else if (next === '&') { out += '$0'; i++; }
        else if (/[1-9]/.test(next || '')) {
            if (/\d/.test(js[i + 2] || '')) fail(where, `two-digit group reference in "${js}"`);
            out += '${' + next + '}';
            i++;
        } else if (next === '`' || next === "'" || next === '<') fail(where, `unsupported "$${next}" in "${js}"`);
        else out += '$$';
    }
    return out;
}

function field(where, name) {
    return FIELDS[name] ? `Field.${FIELDS[name]}` : fail(where, `unknown property "${name}"`);
}

// samples: lowercased corpus user agents, used to pick the most selective prefilter words.
function generate(source, samples) {
    const regexMethods = [];   // [GeneratedRegex] declarations
    const maps = new Map();    // map object -> { name, code }
    const known = new Map([[source.oldSafariMap, 'OldSafariMap'], [source.windowsVersionMap, 'WindowsVersionMap']]);
    const fn = f => f === source.lowerize ? 'lowerize' : f === source.trim ? 'trim' : f === source.strMapper ? 'strMapper' : null;

    // Rule regexes are always /i. Replacement regexes may also be case-sensitive and/or /g
    // (/g is not a .NET option: it becomes the "all" flag of the assignment).
    function regexMethod(name, regex, where, isReplacement) {
        const flags = [...regex.flags].sort().join('');
        const allowed = isReplacement ? ['', 'g', 'i', 'gi'] : ['i'];
        if (!allowed.includes(flags)) fail(where, `unexpected regex flags "${regex.flags}"`);
        checkPattern(where, regex.source);
        const options = regex.ignoreCase ? 'Options' : 'CaseSensitiveOptions';
        regexMethods.push(
            `    [GeneratedRegex(${csPattern(regex.source)}, ${options}, TimeoutMilliseconds, "")]\n` +
            `    private static partial Regex ${name}();`);
        return name;   // a method group: the regex is created on first use
    }

    function stringMap(where, map) {
        if (maps.has(map)) return maps.get(map).name;
        const name = known.get(map) || `${where.replace(/[^A-Za-z0-9]+/g, '_')}_Map`;
        // Object.keys follows the same order as the for...in loop in strMapper.
        const entries = Object.keys(map).map(key => {
            const needles = (Array.isArray(map[key]) ? map[key] : [map[key]]).filter(v => typeof v === 'string');
            const result = key === '?' ? 'null' : csString(key);
            return `        new(${result}, [${needles.map(csString).join(', ')}]),`;
        });
        const fallback = Object.prototype.hasOwnProperty.call(map, '*') ? `, defaultValue: ${csString(map['*'])}` : '';
        const code = `    private static readonly StringMap ${name} = new(\n    [\n${entries.join('\n')}\n    ]${fallback});`;
        maps.set(map, { name, code });
        return name;
    }

    function assignment(where, prop, replaceName) {
        if (typeof prop === 'string') return `Capture(${field(where, prop)})`;
        if (!Array.isArray(prop) || prop.length < 2 || prop.length > 4) fail(where, `unexpected property ${JSON.stringify(prop)}`);

        const target = field(where, prop[0]);
        const [, a, b, c] = prop;
        const isRegex = v => v && typeof v.exec === 'function' && typeof v.test === 'function';

        if (prop.length === 2) {
            if (typeof a !== 'function') return `Constant(${target}, ${csString(a)})`;
            if (fn(a) === 'lowerize') return `Lowercase(${target})`;
            if (fn(a) === 'trim') return `TrimStart(${target})`;
            fail(where, `unknown function in ${prop[0]}`);
        }
        if (prop.length === 3) {
            if (typeof a === 'function' && !isRegex(a)) {
                if (fn(a) !== 'strMapper') fail(where, `unknown mapper function in ${prop[0]}`);
                return `Map(${target}, ${stringMap(where, b)})`;
            }
            if (!isRegex(a)) fail(where, `expected a regex in ${prop[0]}`);
            const regex = regexMethod(replaceName, a, where, true);
            return `Replace(${target}, ${regex}, ${csString(csReplacement(where, b))}, all: ${a.global})`;
        }
        // length 4: [PROP, regex, replacement, function]
        if (!isRegex(a) || fn(c) !== 'lowerize') fail(where, `unexpected 4-part property ${prop[0]}`);
        const regex = regexMethod(replaceName, a, where, true);
        return `ReplaceThenLowercase(${target}, ${regex}, ${csString(csReplacement(where, b))}, all: ${a.global})`;
    }

    // Prefilter words are numbered in order of first use; rules refer to them by number.
    const words = new Map();
    const wordId = w => { if (!words.has(w)) words.set(w, words.size); return words.get(w); };

    const ruleSets = [];
    let ruleCount = 0, regexCount = 0;
    for (const [key, csName] of Object.entries(CATEGORIES)) {
        const table = source.regexes[key];
        if (!table) fail(key, 'missing category');
        const lines = [];
        for (let i = 0; i < table.length; i += 2) {
            const r = i / 2;
            const where = `${csName}_${r}`;
            const regexes = table[i].map((regex, j) => regexMethod(`${where}_${j}`, regex, `${where}/${j}`, false));
            const props = table[i + 1].map((p, k) => assignment(`${where}/prop${k}`, p, `${where}_Replace${k}`));
            const required = table[i].map(regex => requiredWords(regex, samples));
            const needs = required.map(groups =>
                `Needs(${groups.map(group => `[${group.map(wordId).join(', ')}]`).join(', ')})`);
            const readable = required.map(groups => groups.length
                ? groups.map(group => group.map(w => JSON.stringify(w)).join('|')).join(' & ')
                : '(always)').join('; ');
            lines.push(`        // ${r}\n        new([${regexes.join(', ')}],\n            [${props.join(', ')}],\n` +
                `            // needs ${readable.replace(/\*\//g, '* /')}\n            [${needs.join(', ')}]),`);
            ruleCount++;
            regexCount += regexes.length;
        }
        ruleSets.push(`    public static readonly Rule[] ${csName} =\n    [\n${lines.join('\n')}\n    ];`);
    }

    const code = [
        '// <auto-generated>',
        `// Generated by tools/RuleGenerator from tools/RuleGenerator/rules/user-agent-rules.js`,
        `// (uaParser.Net rules ${source.RULES_VERSION}). Do not edit by hand: change that file and`,
        '// run `npm run rules` in tools/RuleGenerator.',
        '//',
        `// The rules are based on ${source.BASED_ON}, MIT License,`,
        '// Copyright (c) 2012-2025 Faisal Salman <f@faisalman.com>.',
        '// </auto-generated>',
        '',
        '#nullable enable',
        '',
        'using System.Text.RegularExpressions;',
        '',
        'using uaParserLibrary.Parsing;',
        '',
        'using static uaParserLibrary.Parsing.Assignment;',
        'using static uaParserLibrary.Parsing.Prefilter;',
        '',
        'namespace uaParserLibrary.Rules;',
        '',
        'internal static partial class UserAgentRules',
        '{',
        `    public const string RulesVersion = ${csString(source.RULES_VERSION)};`,
        `    public const string BasedOn = ${csString(source.BASED_ON)};`,
        '',
        '    // JavaScript regex semantics, case-insensitive with the invariant culture (the "" below).',
        '    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.ECMAScript;',
        '    private const RegexOptions CaseSensitiveOptions = RegexOptions.ECMAScript;',
        '    private const int TimeoutMilliseconds = Rule.TimeoutMilliseconds;',
        '',
        '    // Lowercase words the prefilters refer to by index.',
        `    public static readonly string[] PrefilterWords =\n    [\n${[...words.keys()].map((w, n) => `        ${csString(w)}, // ${n}`).join('\n')}\n    ];`,
        '',
        '    // String maps come first: static fields are initialised in order.',
        [...maps.values()].map(m => m.code).join('\n\n'),
        '',
        ruleSets.join('\n\n'),
        '',
        regexMethods.join('\n\n'),
        '}',
        '',
    ].join('\n');

    return { code, ruleCount, regexCount: regexMethods.length, matchRegexCount: regexCount, mapCount: maps.size };
}

// The prefilters must never skip a regex that would match. Check every regex against every
// all-ASCII corpus user agent (the only input the prefilters are used for).
function verifyPrefilters(source, userAgents, samples) {
    const passes = (groups, lowerUa) => groups.every(g => g.some(w => lowerUa.includes(w)));
    const inputs = userAgents.map(ua => source.trim(ua, source.UA_MAX_LENGTH)).filter(ua => /^[\x00-\x7f]*$/.test(ua));
    let checks = 0;
    for (const key of Object.keys(CATEGORIES)) {
        const table = source.regexes[key];
        for (let i = 0; i < table.length; i += 2) {
            for (const regex of table[i]) {
                const groups = requiredWords(regex, samples);
                for (const ua of inputs) {
                    checks++;
                    if (regex.test(ua) && !passes(groups, ua.toLowerCase())) {
                        fail(`${key} rule ${i / 2}`, `prefilter ${JSON.stringify(groups)} rejects a match of /${regex.source}/ in "${ua}"`);
                    }
                }
            }
        }
    }
    return checks;
}

module.exports = { generate, verifyPrefilters, csString };
