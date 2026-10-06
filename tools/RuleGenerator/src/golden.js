'use strict';

// Builds the answer key: what ua-parser-js returns for every user agent in the corpus.
// "undefined" in JavaScript is written as null.

const { CATEGORIES, upstreamCases, userAgents } = require('./corpus');

const FIELDS = {
    browser: ['name', 'version', 'major'],
    cpu: ['architecture'],
    device: ['vendor', 'model', 'type'],
    engine: ['name', 'version'],
    os: ['name', 'version'],
};

function parse(UAParser, ua) {
    const result = new UAParser(ua).getResult();
    const entry = { ua };
    for (const category of CATEGORIES) {
        entry[category] = {};
        for (const field of FIELDS[category]) {
            const value = result[category][field];
            entry[category][field] = value === undefined ? null : value;
        }
    }
    return entry;
}

function build(pristine, patched, version) {
    const cases = userAgents().map(({ ua }) => parse(pristine, ua));

    // The copy we evaluate to read the internals must behave exactly like the package.
    const patchedDiffers = cases.filter(c => JSON.stringify(parse(patched, c.ua)) !== JSON.stringify(c));
    if (patchedDiffers.length) {
        throw new Error(`Patched parser differs from the package on ${patchedDiffers.length} user agents.`);
    }

    return {
        generator: 'tools/RuleGenerator',
        upstream: `ua-parser-js ${version} (MIT)`,
        count: cases.length,
        cases,
    };
}

// ua-parser-js must pass its own tests, otherwise it is not a trustworthy answer key.
function selfCheck(UAParser) {
    const failures = [];
    for (const c of upstreamCases()) {
        const actual = new UAParser(c.ua).getResult()[c.category];
        // Same rule as upstream's test.js: "undefined" or a missing key means no value.
        for (const field of FIELDS[c.category]) {
            const expected = c.expect[field];
            const want = expected === 'undefined' ? undefined : expected;
            if (actual[field] !== want) {
                failures.push(`${c.category} "${c.desc}" ${field}: expected ${want}, got ${actual[field]}`);
            }
        }
    }
    return failures;
}

module.exports = { FIELDS, build, selfCheck };
