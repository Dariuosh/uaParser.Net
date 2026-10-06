'use strict';

// Collects every user agent we test against, without duplicates, in a stable order:
//   1. ua-parser-js's own test data (with its expected values),
//   2. user agents from the uaParser.Net 1.x tests and samples,
//   3. extra, more recent user agents.

const fs = require('fs');
const path = require('path');

const CORPUS_DIR = path.join(__dirname, '..', 'corpus');
const UPSTREAM_DIR = path.join(CORPUS_DIR, 'ua-parser-js-1.0.41');
const CATEGORIES = ['browser', 'cpu', 'device', 'engine', 'os'];

// One user agent per line, kept exactly as written (no trimming); '#' lines are comments.
function readLines(file) {
    return fs.readFileSync(file, 'utf8')
        .split(/\r?\n/)
        .filter(line => line.trim() && !line.trimStart().startsWith('#'));
}

// Upstream test cases: { category, desc, ua, expect }.
function upstreamCases() {
    return CATEGORIES.flatMap(category =>
        JSON.parse(fs.readFileSync(path.join(UPSTREAM_DIR, `${category}-test.json`), 'utf8'))
            .filter(c => c.ua)
            .map(c => ({ category, desc: c.desc, ua: c.ua, expect: c.expect })));
}

function userAgents() {
    const seen = new Set();
    const list = [];
    const add = (ua, source) => {
        if (!seen.has(ua)) {
            seen.add(ua);
            list.push({ ua, source });
        }
    };

    for (const c of upstreamCases()) add(c.ua, 'ua-parser-js');
    for (const ua of readLines(path.join(CORPUS_DIR, 'legacy-port-user-agents.txt'))) add(ua, 'legacy');
    for (const ua of readLines(path.join(CORPUS_DIR, 'extra-user-agents.txt'))) add(ua, 'extra');
    return list;
}

module.exports = { CATEGORIES, upstreamCases, userAgents };
