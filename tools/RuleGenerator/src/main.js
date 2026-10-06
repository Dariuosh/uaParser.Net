'use strict';

// Usage: node src/main.js <rules|bots|golden|coverage|differences|samples|all>

const fs = require('fs');
const path = require('path');
const source = require('../rules/user-agent-rules');
const botRules = require('../rules/bot-rules');
const upstream = require('./upstream');
const rules = require('./rules');
const bots = require('./bots');
const samples = require('./samples');
const golden = require('./golden');
const coverage = require('./coverage');
const differences = require('./differences');
const { userAgents } = require('./corpus');

const REPO_ROOT = path.join(__dirname, '..', '..', '..');
const RULES_FILE = path.join(REPO_ROOT, 'uaParserLibrary', 'Rules', 'UserAgentRules.g.cs');
const BOT_RULES_FILE = path.join(REPO_ROOT, 'uaParserLibrary', 'Rules', 'BotRules.g.cs');
const GOLDEN_FILE = path.join(REPO_ROOT, 'uaParserTest', 'TestData', 'expected-results.json');
const GOLDEN_BOTS_FILE = path.join(REPO_ROOT, 'uaParserTest', 'TestData', 'expected-bots.json');
const COVERAGE_FILE = path.join(__dirname, '..', 'reports', 'coverage.md');
const DIFFERENCES_FILE = path.join(__dirname, '..', 'reports', 'differences.md');
const SAMPLES_FILE = path.join(REPO_ROOT, 'Shared', 'SampleUserAgents.g.cs');

function write(file, content) {
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, content.endsWith('\n') ? content : content + '\n');
    console.log(`wrote ${path.relative(REPO_ROOT, file)}`);
}

function runRules() {
    const corpus = userAgents().map(c => c.ua);
    const samples = corpus.map(ua => source.trim(ua, source.UA_MAX_LENGTH).toLowerCase());
    const checks = rules.verifyPrefilters(source, corpus, samples);
    console.log(`prefilters: sound on ${checks} regex/user-agent checks`);
    const result = rules.generate(source, samples);
    write(RULES_FILE, result.code);
    console.log(`rules: ${result.ruleCount} rules, ${result.matchRegexCount} match regexes, ` +
        `${result.regexCount} generated regexes in total, ${result.mapCount} string maps`);
}

// The examples of the bot list, and every corpus user agent, as the parser sees them.
function botTexts() {
    const seen = new Set();
    for (const ua of [...botRules.examples(), ...userAgents().map(c => c.ua)]) seen.add(source.parse(ua).ua);
    return [...seen];
}

function runBots() {
    const result = bots.generate(botTexts());
    write(BOT_RULES_FILE, result.code);
    console.log(`bots: ${result.botCount} bots, ${result.textCount} found by text, ${result.regexCount} by regex`);

    const seen = new Set();
    const cases = [];
    for (const ua of botRules.examples()) {
        if (seen.has(ua)) continue;
        seen.add(ua);
        cases.push({ ua, bot: bots.expected(source.parse(ua).ua) });
    }
    const header = {
        generator: 'tools/RuleGenerator',
        bots: `${botRules.SOURCE.name} ${botRules.SOURCE.commit.slice(0, 12)} with rules/bot-rules.js`,
        count: cases.length,
    };
    const json = JSON.stringify(header, null, 2).replace(/\n}$/, ',\n  "cases": [\n') +
        cases.map(c => `    ${JSON.stringify(c)}`).join(',\n') + '\n  ]\n}';
    write(GOLDEN_BOTS_FILE, json);
    console.log(`expected bots: ${cases.length} user agents`);
}

function runSamples() {
    const result = samples.generate(rules.csString);
    write(SAMPLES_FILE, result.code);
    console.log(`samples: ${result.count} user agents in ${result.groupCount} groups`);
}

function runGolden() {
    const failures = golden.selfCheck(source);
    if (failures.length) {
        console.error(failures.join('\n'));
        throw new Error(`The rules fail ${failures.length} ua-parser-js test expectations that are not recorded as deliberate.`);
    }
    const { cases, ...header } = golden.build(source);
    // One user agent per line keeps future diffs readable.
    const json = JSON.stringify(header, null, 2).replace(/\n}$/, ',\n  "cases": [\n') +
        cases.map(c => `    ${JSON.stringify(c)}`).join(',\n') + '\n  ]\n}';
    write(GOLDEN_FILE, json);
    console.log(`expected results: ${cases.length} user agents`);
}

function runCoverage() {
    const { markdown, total, covered, uncovered } = coverage.report(source, coverage.analyze(source));
    write(COVERAGE_FILE, markdown);
    console.log(`coverage: ${covered} of ${total} regexes reached`);
    if (uncovered) {
        console.warn(`warning: ${uncovered} regexes have no test user agent; add one to corpus/extra-user-agents.txt`);
    }
}

function runDifferences() {
    const { markdown, count, agents } = differences.report(source, upstream);
    write(DIFFERENCES_FILE, markdown);
    console.log(`differences from ua-parser-js ${upstream.version}: ${count} values, ${agents} user agents`);
}

const command = process.argv[2] || 'all';
console.log(`uaParser.Net rules ${source.RULES_VERSION}, based on ${source.BASED_ON}`);

if (command === 'rules' || command === 'all') runRules();
if (command === 'bots' || command === 'all') runBots();
if (command === 'golden' || command === 'all') runGolden();
if (command === 'coverage' || command === 'all') runCoverage();
if (command === 'differences' || command === 'all') runDifferences();
if (command === 'samples' || command === 'all') runSamples();
if (!['rules', 'bots', 'golden', 'coverage', 'differences', 'samples', 'all'].includes(command)) {
    console.error(`Unknown command "${command}". Use rules, bots, golden, coverage, differences, samples or all.`);
    process.exit(1);
}
