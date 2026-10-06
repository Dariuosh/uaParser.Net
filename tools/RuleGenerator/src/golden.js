'use strict';

// Builds the expected results: what uaParser.Net's rules give for every user agent in the
// corpus, worked out in JavaScript by rules/user-agent-rules.js. The C# parser must give exactly
// the same, field by field. "undefined" in JavaScript is written as null.

const fs = require('fs');
const path = require('path');
const { CATEGORIES, upstreamCases, userAgents } = require('./corpus');

const FIELDS = {
    browser: ['name', 'version', 'major'],
    cpu: ['architecture'],
    device: ['vendor', 'model', 'type'],
    engine: ['name', 'version'],
    os: ['name', 'version'],
};

// ua-parser-js test cases whose result uaParser.Net changes on purpose, with the new value.
const DIFFERENCES_FILE = path.join(__dirname, '..', 'corpus', 'upstream-test-differences.json');

function entry(source, ua) {
    const result = source.parse(ua);
    const item = { ua };
    for (const category of CATEGORIES) {
        item[category] = {};
        for (const field of FIELDS[category]) {
            const value = result[category][field];
            item[category][field] = value === undefined ? null : value;
        }
    }
    return item;
}

function build(source) {
    const cases = userAgents().map(({ ua }) => entry(source, ua));
    return {
        generator: 'tools/RuleGenerator',
        rules: `uaParser.Net rules ${source.RULES_VERSION}, based on ${source.BASED_ON}`,
        count: cases.length,
        cases,
    };
}

function intendedDifferences() {
    return JSON.parse(fs.readFileSync(DIFFERENCES_FILE, 'utf8'));
}

// The rules must still pass the test cases of ua-parser-js 1.0.41, except where
// corpus/upstream-test-differences.json records a deliberate change (and its reason).
function selfCheck(source) {
    const differences = intendedDifferences();
    const key = (category, desc, field) => `${category}\u0000${desc}\u0000${field}`;
    const allowed = new Map(differences.map(d => [key(d.category, d.desc, d.field), d]));
    const used = new Set();
    const failures = [];

    for (const c of upstreamCases()) {
        const actual = source.parse(c.ua)[c.category];
        // Same rule as upstream's test.js: "undefined" or a missing key means no value.
        for (const field of FIELDS[c.category]) {
            const expected = c.expect[field];
            const want = expected === undefined || expected === 'undefined' ? undefined : expected;
            if (actual[field] === want) continue;

            const k = key(c.category, c.desc, field);
            const difference = allowed.get(k);
            const ours = actual[field] === undefined ? null : actual[field];
            if (difference && difference.value === ours) {
                used.add(k);
            } else {
                failures.push(`${c.category} "${c.desc}" ${field}: ua-parser-js expects ${want}, the rules give ${actual[field]}` +
                    (difference ? ` (upstream-test-differences.json says ${difference.value})` : ''));
            }
        }
    }

    for (const d of differences) {
        if (!used.has(key(d.category, d.desc, d.field))) {
            failures.push(`upstream-test-differences.json: "${d.desc}" ${d.category}.${d.field} is no longer a difference; remove it.`);
        }
    }
    return failures;
}

module.exports = { FIELDS, build, selfCheck };
