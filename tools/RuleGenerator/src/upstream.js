'use strict';

// ua-parser-js 1.0.41, the library uaParser.Net's rules were copied from. It is no longer the
// source of the rules (see rules/user-agent-rules.js); it is used only to report where the
// results differ (reports/differences.md) and for npm run bench.

const UAParser = require('ua-parser-js');

module.exports = { UAParser, version: UAParser.VERSION };
