'use strict';

// Usage: node src/bench.js [--out <folder>]
// Measures ua-parser-js on the same user agents and in the same way as
// `dotnet run -c Release -- --quick` in uaParserBenchmark, so the numbers can be compared.
// Prints the results and saves them as Markdown and JSON in benchmark-results/ at the
// repository root.

const fs = require('fs');
const path = require('path');
const { upstreamCases } = require('./corpus');

const CORPUS_DIR = path.join(__dirname, '..', 'corpus');
const lines = file => fs.readFileSync(path.join(CORPUS_DIR, file), 'utf8')
    .split(/\r?\n/).filter(l => l.trim() && !l.trimStart().startsWith('#'));

const corpora = [
    ['1.x samples', lines('legacy-port-user-agents.txt')],
    ['recent', lines('extra-user-agents.txt')],
    ['ua-parser-js tests', [...new Set(upstreamCases().map(c => c.ua))]],
];

const now = () => Number(process.hrtime.bigint()) / 1000;   // µs
const sleep = ms => Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, ms);

let start = now();
const UAParser = require('ua-parser-js');
new UAParser(corpora[0][1][0]).getResult();
const cold = (now() - start) / 1000;

const parse = uas => { for (const ua of uas) new UAParser(ua).getResult(); };

const rows = [];
for (const [name, uas] of corpora) {
    for (let r = 0; r < 40; r++) parse(uas);
    sleep(500);
    for (let r = 0; r < 10; r++) parse(uas);
    const trials = [];
    for (let t = 0; t < 15; t++) {
        start = now();
        for (let r = 0; r < 3; r++) parse(uas);
        trials.push((now() - start) / (3 * uas.length));
    }
    trials.sort((a, b) => a - b);
    rows.push({ corpus: name, userAgents: uas.length, minMicroseconds: +trials[0].toFixed(1), medianMicroseconds: +trials[7].toFixed(1) });
}

const os = require('os');
const machine = {
    cpu: (os.cpus()[0] || {}).model || os.arch(),
    cpus: String(os.cpus().length),
    os: `${os.type()} ${os.release()}`,
    architecture: os.arch(),
    runtime: `Node ${process.version}`,
};
const date = new Date();
const report = [
    '# ua-parser-js quick benchmark',
    '',
    `- Date: ${date.toISOString()}`,
    `- Library: ua-parser-js ${UAParser.VERSION}`,
    `- Runtime: ${machine.runtime}`,
    `- Machine: ${machine.cpu}, ${machine.cpus} CPUs, ${machine.os} (${machine.architecture})`,
    '',
    `First call (cold start, including require): **${cold.toFixed(0)} ms**`,
    '',
    'Per user agent, after warm-up (15 trials):',
    '',
    '| Corpus | User agents | min (µs) | median (µs) |',
    '|---|---:|---:|---:|',
    ...rows.map(r => `| ${r.corpus} | ${r.userAgents} | ${r.minMicroseconds.toFixed(1)} | ${r.medianMicroseconds.toFixed(1)} |`),
    '',
].join('\n');
console.log(report);

function resultsFolder() {
    const i = process.argv.indexOf('--out');
    if (i >= 0 && process.argv[i + 1]) return path.resolve(process.argv[i + 1]);
    for (let dir = process.cwd(); ; dir = path.dirname(dir)) {
        if (fs.existsSync(path.join(dir, 'uaParser.slnx'))) return path.join(dir, 'benchmark-results');
        if (path.dirname(dir) === dir) return path.join(process.cwd(), 'benchmark-results');
    }
}
const folder = resultsFolder();
fs.mkdirSync(folder, { recursive: true });
const pad = n => String(n).padStart(2, '0');
const stamp = `${date.getFullYear()}${pad(date.getMonth() + 1)}${pad(date.getDate())}-${pad(date.getHours())}${pad(date.getMinutes())}${pad(date.getSeconds())}`;
const stem = path.join(folder, `ua-parser-js-quick-${stamp}`);
fs.writeFileSync(`${stem}.md`, report);
fs.writeFileSync(`${stem}.json`, JSON.stringify({
    library: 'ua-parser-js', version: UAParser.VERSION, date: date.toISOString(), machine,
    coldStartMilliseconds: +cold.toFixed(1), perUserAgent: rows,
}, null, 2) + '\n');
console.log(`Saved: ${stem}.md and .json`);
