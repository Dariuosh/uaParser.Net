'use strict';

// Loads ua-parser-js and exposes the internals the generator needs.
//
// The rule table (`regexes`) and its helper functions are private to the
// library's closure, so we evaluate a copy of the source with one extra line
// that publishes them. The library itself is not modified on disk.

const fs = require('fs');
const path = require('path');
const vm = require('vm');

const PACKAGE_DIR = path.dirname(require.resolve('ua-parser-js/package.json'));
const SOURCE_FILE = path.join(PACKAGE_DIR, 'src', 'ua-parser.js');

const ANCHOR = 'UAParser.VERSION = LIBVERSION;';
const EXPOSE = [
    'regexes', 'rgxMapper', 'strMapper', 'lowerize', 'majorize', 'trim',
    'oldSafariMap', 'windowsVersionMap', 'UA_MAX_LENGTH', 'LIBVERSION',
];

function load() {
    const pkg = JSON.parse(fs.readFileSync(path.join(PACKAGE_DIR, 'package.json'), 'utf8'));
    const source = fs.readFileSync(SOURCE_FILE, 'utf8');

    const occurrences = source.split(ANCHOR).length - 1;
    if (occurrences !== 1) {
        throw new Error(`Expected "${ANCHOR}" exactly once in ${SOURCE_FILE}, found ${occurrences}. ` +
            'The upstream source layout changed; update upstream.js.');
    }

    const expose = `UAParser.__internals = { ${EXPOSE.map(n => `${n}: ${n}`).join(', ')} };`;
    const patched = source.replace(ANCHOR, `${ANCHOR}\n${expose}`);

    const sandbox = { module: { exports: {} } };
    sandbox.exports = sandbox.module.exports;
    vm.runInNewContext(patched, sandbox, { filename: SOURCE_FILE });

    const UAParser = sandbox.module.exports;
    const internals = UAParser.__internals;
    if (!internals || !internals.regexes) {
        throw new Error('Could not read the ua-parser-js internals.');
    }
    if (internals.LIBVERSION !== pkg.version) {
        throw new Error(`Source says ${internals.LIBVERSION}, package.json says ${pkg.version}.`);
    }

    return { version: pkg.version, license: pkg.license, UAParser, ...internals };
}

// The parser exactly as consumers get it (unpatched), used for the answer key.
function pristineParser() {
    return require('ua-parser-js');
}

module.exports = { load, pristineParser, SOURCE_FILE };
