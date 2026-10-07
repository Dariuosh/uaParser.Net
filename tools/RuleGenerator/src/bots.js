'use strict';

// Turns the bot rules (rules/bot-rules.js and rules/crawler-user-agents.json) into C#
// (uaParserLibrary/Rules/BotRules.g.cs) and writes what they give for the list's examples
// (uaParserTest/TestData/expected-bots.json).
//
// Most patterns are plain text: the C# detector finds all of them with one multi-string search.
// The others become [GeneratedRegex] methods, each skipped unless the user agent contains words
// every match must contain. Everything is checked here before it is written.

const botRules = require('../rules/bot-rules');
const { csString, csPattern, checkPattern } = require('./rules');
const { requiredWords } = require('./prefilter');

const TAB = '\t';

function fail(message) {
    throw new Error(`bots: ${message}`);
}

// How the C# detector finds each bot: a plain text (with where it must be) or a regex.
function plan() {
    const regexes = [];
    const bots = botRules.bots().map((bot, index) => {
        if (botRules.PLAIN.test(bot.pattern)) {
            const atStart = bot.pattern.startsWith('^');
            const atEnd = /[^\\]\$$/.test(bot.pattern);
            const text = botRules.literal(bot.pattern);
            const mark = atStart && atEnd ? '!' : atStart ? '^' : atEnd ? '$' : '=';
            return { ...bot, index, find: { kind: 'text', text, atStart, atEnd }, code: mark + text };
        }
        checkPattern(`bot ${index}`, bot.pattern);
        const n = regexes.length;
        regexes.push({ bot: index, regex: bot.regex });
        return { ...bot, index, find: { kind: 'regex', n }, code: `~${n}` };
    });
    return { bots, regexes };
}

// Where a plain text pattern matches, as a regex would: { index, length } or null.
function findText(find, text) {
    if (find.atStart && find.atEnd) return text === find.text ? { index: 0, length: text.length } : null;
    if (find.atStart) return text.startsWith(find.text) ? { index: 0, length: find.text.length } : null;
    if (find.atEnd) return text.endsWith(find.text) ? { index: text.length - find.text.length, length: find.text.length } : null;
    const at = text.indexOf(find.text);
    return at < 0 ? null : { index: at, length: find.text.length };
}

// texts: user agents as the parser sees them. samples: the same, lowercased.
function verify(model, texts, samples, fallbackNeeds) {
    // 1. A plain text search finds exactly what the pattern's regex finds.
    for (const bot of model.bots) {
        if (bot.find.kind !== 'text') continue;
        for (const text of texts) {
            const m = bot.regex.exec(text);
            const found = findText(bot.find, text);
            if (!m !== !found || (m && (m.index !== found.index || m[0].length !== found.length))) {
                fail(`the text "${bot.find.text}" and the pattern /${bot.pattern}/ disagree on "${text}"`);
            }
        }
    }

    // 2. A regex is only skipped when it cannot match (checked on all-ASCII text, the only
    // text the words are used for).
    const passes = (groups, lower) => groups.every(g => g.some(w => lower.includes(w)));
    const ascii = texts.filter(t => /^[\x00-\x7f]*$/.test(t));
    const gated = [...model.regexes.map(r => ({ regex: r.regex, needs: r.needs })),
        { regex: botRules.FALLBACK, needs: fallbackNeeds }];
    for (const { regex, needs } of gated) {
        for (const text of ascii) {
            if (regex.test(text) && !passes(needs, text.toLowerCase())) {
                fail(`the words ${JSON.stringify(needs)} would skip a match of /${regex.source}/ in "${text}"`);
            }
        }
    }

    // 3. Every example of every bot is found (as that bot or a bot that matches earlier).
    for (const bot of model.bots) {
        for (const example of bot.instances) {
            if (!botRules.detect(example)) fail(`the example "${example}" of /${bot.pattern}/ is not found`);
        }
    }

    // 4. The C# table uses tabs and line breaks as separators, and the file stays ASCII.
    for (const bot of model.bots) {
        for (const value of [bot.code, bot.name, bot.category || '', bot.url || '']) {
            if (/[\t\r\n]/.test(value) || !/^[\x20-\x7e]*$/.test(value)) fail(`unexpected character in ${JSON.stringify(value)}`);
            if (value === '-') fail(`"-" means "none" in the table: ${bot.pattern}`);
        }
    }
}

function words(groups) {
    return `[${groups.map(g => `[${g.map(csString).join(', ')}]`).join(', ')}]`;
}

function generate(texts) {
    const samples = texts.map(t => t.toLowerCase());
    const model = plan();
    for (const r of model.regexes) r.needs = requiredWords(r.regex, samples);
    const fallbackNeeds = requiredWords(botRules.FALLBACK, samples);
    // The C# detector skips every regex when the user agent has none of their first words.
    for (const r of [...model.regexes, { regex: botRules.FALLBACK, needs: fallbackNeeds }]) {
        if (!r.needs.length) fail(`no word that every match of /${r.regex.source}/ contains; make the pattern more specific`);
    }
    if (botRules.FALLBACK.flags !== 'i') fail('the fallback must be /i and nothing else');
    verify(model, texts, samples, fallbackNeeds);

    const lines = model.bots.map(b => [b.code, b.name, b.category || '-', b.url || '-'].join(TAB));
    const patterns = model.regexes.map((r, n) =>
        `        new(${r.bot}, Pattern${n}, ${words(r.needs)}), // ${model.bots[r.bot].name}`);
    const methods = model.regexes.map((r, n) =>
        `    [GeneratedRegex(${csPattern(r.regex.source)}, Options, TimeoutMilliseconds, "")]\n` +
        `    private static partial Regex Pattern${n}();`);
    const source = botRules.SOURCE;

    const code = [
        '// <auto-generated>',
        '// Generated by tools/RuleGenerator from tools/RuleGenerator/rules/bot-rules.js and',
        `// ${source.name} (${source.url}, commit ${source.commit.slice(0, 12)}),`,
        '// MIT License, Copyright (c) 2017 Martin Monperrus. Do not edit by hand: change',
        '// bot-rules.js and run `npm run bots` in tools/RuleGenerator.',
        '// </auto-generated>',
        '',
        '#nullable enable',
        '',
        'using System.Text.RegularExpressions;',
        '',
        'using uaParserLibrary.Parsing;',
        '',
        'namespace uaParserLibrary.Rules;',
        '',
        'internal static partial class BotRules',
        '{',
        `    public const string Source = ${csString(`${source.name} ${source.commit.slice(0, 12)}`)};`,
        '',
        '    // The patterns are case-sensitive and use JavaScript regex semantics.',
        '    private const RegexOptions Options = RegexOptions.ECMAScript;',
        '    private const int TimeoutMilliseconds = Rule.TimeoutMilliseconds;',
        '',
        '    // One bot per line, in list order: how it is found, its name, category and web page',
        '    // ("-" for none), separated by tabs. How it is found: "=text" anywhere, "^text" at the',
        '    // start, "$text" at the end, "!text" the whole user agent, "~n" Patterns[n].',
        `    public const string Table = """\n${lines.map(l => '        ' + l).join('\n')}\n        """;`,
        '',
        '    // The patterns that are not plain text: the bot (line of the table), the regex, and',
        '    // lowercase words every match contains (for each group, one of its words).',
        `    public static readonly BotPattern[] Patterns =\n    [\n${patterns.join('\n')}\n    ];`,
        '',
        '    // Unknown bots: a word that ends in bot, crawler, spider or scraper (not Cubot, a phone',
        '    // maker). The name is the word.',
        `    public static readonly string[][] FallbackNeeds = ${words(fallbackNeeds)};`,
        '',
        `    [GeneratedRegex(${csPattern(botRules.FALLBACK.source)}, RegexOptions.IgnoreCase | Options, TimeoutMilliseconds, "")]`,
        '    public static partial Regex Fallback();',
        '',
        methods.join('\n\n'),
        '}',
        '',
    ].join('\n');

    return {
        code,
        botCount: model.bots.length,
        textCount: model.bots.length - model.regexes.length,
        regexCount: model.regexes.length,
    };
}

// The expected result for a user agent (as the parser sees it): { name, category, url } or null.
function expected(text) {
    const bot = botRules.detect(text);
    return bot && { name: bot.name, category: bot.category, url: bot.url };
}

module.exports = { generate, expected };
