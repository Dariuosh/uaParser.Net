'use strict';

// Usage: node src/main.js <golden|coverage|all>

const fs = require('fs');
const path = require('path');
const upstream = require('./upstream');
const golden = require('./golden');
const coverage = require('./coverage');

const REPO_ROOT = path.join(__dirname, '..', '..', '..');
const GOLDEN_FILE = path.join(REPO_ROOT, 'uaParserTest', 'TestData', 'ua-parser-js.golden.json');
const COVERAGE_FILE = path.join(__dirname, '..', 'reports', 'coverage.md');

function write(file, content) {
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, content.endsWith('\n') ? content : content + '\n');
    console.log(`wrote ${path.relative(REPO_ROOT, file)}`);
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

if (command === 'golden' || command === 'all') runGolden(up);
if (command === 'coverage' || command === 'all') runCoverage(up);
if (!['golden', 'coverage', 'all'].includes(command)) {
    console.error(`Unknown command "${command}". Use golden, coverage or all.`);
    process.exit(1);
}
