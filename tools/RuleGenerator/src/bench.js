'use strict';

// Usage: node src/bench.js
// Measures ua-parser-js on the same user agents and in the same way as
// `dotnet run -c Release -- --quick` in uaParserBenchmark, so the numbers can be compared.

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

console.log(`ua-parser-js ${UAParser.VERSION} quick benchmark: Node ${process.version}, ${require('os').cpus().length} CPUs`);
console.log(`First call (cold start, including require): ${cold.toFixed(0)} ms`);
console.log('');
console.log('Per user agent, after warm-up (15 trials):');
console.log(`${'Corpus'.padEnd(20)} ${'User agents'.padStart(11)} ${'min'.padStart(9)} ${'median'.padStart(9)}`);

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
    console.log(`${name.padEnd(20)} ${String(uas.length).padStart(11)} ${trials[0].toFixed(1).padStart(6)} µs ${trials[7].toFixed(1).padStart(6)} µs`);
}
