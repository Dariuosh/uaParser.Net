'use strict';

// Usage: node src/main.js <rules|golden|coverage|samples|all>

const fs = require('fs');
const path = require('path');
const upstream = require('./upstream');
const rules = require('./rules');
const samples = require('./samples');
const golden = require('./golden');
const coverage = require('./coverage');
const { userAgents } = require('./corpus');

const REPO_ROOT = path.join(__dirname, '..', '..', '..');
const RULES_FILE = path.join(REPO_ROOT, 'uaParserLibrary', 'Rules', 'UserAgentRules.g.cs');
const GOLDEN_FILE = path.join(REPO_ROOT, 'uaParserTest', 'TestData', 'ua-parser-js.golden.json');
const COVERAGE_FILE = path.join(__dirname, '..', 'reports', 'coverage.md');
const SAMPLES_FILE = path.join(REPO_ROOT, 'Shared', 'SampleUserAgents.g.cs');

function write(file, content) {
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, content.endsWith('\n') ? content : content + '\n');
    console.log(`wrote ${path.relative(REPO_ROOT, file)}`);
}

function runRules(up) {
    const corpus = userAgents().map(c => c.ua);
    const samples = corpus.map(ua => up.trim(ua, up.UA_MAX_LENGTH).toLowerCase());
    const checks = rules.verifyPrefilters(up, corpus, samples);
    console.log(`prefilters: sound on ${checks} regex/user-agent checks`);
    const result = rules.generate(up, samples);
    write(RULES_FILE, result.code);
    console.log(`rules: ${result.ruleCount} rules, ${result.matchRegexCount} match regexes, ` +
        `${result.regexCount} generated regexes in total, ${result.mapCount} string maps`);
}

function runSamples() {
    const result = samples.generate(rules.csString);
    write(SAMPLES_FILE, result.code);
    console.log(`samples: ${result.count} user agents in ${result.groupCount} groups`);
}

function runGolden(up) {
    const pristine = upstream.pristineParser();
    const failures = golden.selfCheck(pristine);
    if (failures.length) {
        console.error(failures.join('\n'));
        throw new Error(`ua-parser-js ${up.version} fails ${failures.length} of its own test expectations.`);
    }
    const { cases, ...header } = golden.build(pristine, up.UAParser, up.version);
    // One user agent per line keeps future diffs readable.
    const json = JSON.stringify(header, null, 2).replace(/\n}$/, ',\n  "cases": [\n') +
        cases.map(c => `    ${JSON.stringify(c)}`).join(',\n') + '\n  ]\n}';
    write(GOLDEN_FILE, json);
    console.log(`answer key: ${cases.length} user agents`);
}

function runCoverage(up) {
    const { markdown, total, covered, uncovered } = coverage.report(up, coverage.analyze(up));
    write(COVERAGE_FILE, markdown);
    console.log(`coverage: ${covered} of ${total} regexes reached`);
    if (uncovered) {
        console.warn(`warning: ${uncovered} regexes have no test user agent; add one to corpus/extra-user-agents.txt`);
    }
}

const command = process.argv[2] || 'all';
const up = upstream.load();
console.log(`ua-parser-js ${up.version} (${up.license})`);

if (command === 'rules' || command === 'all') runRules(up);
if (command === 'golden' || command === 'all') runGolden(up);
if (command === 'coverage' || command === 'all') runCoverage(up);
if (command === 'samples' || command === 'all') runSamples();
if (!['rules', 'golden', 'coverage', 'samples', 'all'].includes(command)) {
    console.error(`Unknown command "${command}". Use rules, golden, coverage, samples or all.`);
    process.exit(1);
}
