# RuleGenerator

Builds uaParser.Net's parsing data from its rules: `rules/user-agent-rules.js` (browser, engine,
operating system, CPU and device) and `rules/bot-rules.js` (bots).

The rules began as a copy of the rules of [ua-parser-js](https://github.com/faisalman/ua-parser-js)
**1.0.41** (MIT License, copyright (c) 2012-2025 Faisal Salman) and are now maintained here.
Every change since then is marked `uaParser.Net:` in the file, and `reports/differences.md` lists
every user agent whose results differ from ua-parser-js 1.0.41.

Never copy rules, code or test data from ua-parser-js 2.x: it is licensed under AGPL-3.0.

## Requirements

Node.js 18 or later. Node is only needed to regenerate the data; building and using
uaParser.Net needs .NET only.

## Run

```
cd tools/RuleGenerator
npm ci --ignore-scripts
npm run all
```

`--ignore-scripts` stops npm from running package install scripts. The exact version and
integrity hash of ua-parser-js (used only for the differences report and `npm run bench`) are
pinned in `package-lock.json`. (In October 2021 three published versions of ua-parser-js, 0.7.29,
0.8.0 and 1.0.0, were hijacked and contained malware.)

| Command | Output |
|---|---|
| `npm run rules` | `uaParserLibrary/Rules/UserAgentRules.g.cs`: the rules as C# |
| `npm run bots` | `uaParserLibrary/Rules/BotRules.g.cs`: the bot rules as C#, and `uaParserTest/TestData/expected-bots.json`: the bot every example of the bot list belongs to |
| `npm run golden` | `uaParserTest/TestData/expected-results.json`: what the rules (and the bot rules) give for every user agent in the corpus, worked out in JavaScript (the C# parser must give exactly the same) |
| `npm run coverage` | `reports/coverage.md`: which regexes the corpus reaches |
| `npm run differences` | `reports/differences.md`: where the results differ from ua-parser-js 1.0.41 |
| `npm run samples` | `Shared/SampleUserAgents.g.cs`: the example user agents the console sample and the demos use, grouped by the headings in `corpus/extra-user-agents.txt` |
| `npm run all` | All of the above |
| `npm run bench` | Speed of ua-parser-js on this machine (compare with `uaParserBenchmark`) |

The output is deterministic: running it twice gives identical files. CI runs `npm run all` and
fails if the committed files differ.

## Changing a rule

1. Add user agents that show the problem to `corpus/extra-user-agents.txt` (under the right
   `# Heading`).
2. Change `rules/user-agent-rules.js`: keep JavaScript regex syntax, and mark the change with a
   `// uaParser.Net:` comment that says why. Raise `RULES_VERSION`.
3. Run `npm run all`, then review the changes to `expected-results.json` and
   `reports/differences.md`: every changed value must be intended.
4. If a test case of ua-parser-js 1.0.41 now gives a different value on purpose, record it in
   `corpus/upstream-test-differences.json` with the reason; the generator and `UpstreamTests`
   stop on any other difference.
5. Run the .NET tests.

## Bots

The bots come from [crawler-user-agents](https://github.com/monperrus/crawler-user-agents)
(MIT License, copyright (c) 2017 Martin Monperrus): `rules/crawler-user-agents.json` is an
unchanged copy of its `crawler-user-agents.json` (the commit is in `rules/bot-rules.js`).
`rules/bot-rules.js` holds everything uaParser.Net changes:

- `REPLACE`: patterns that also match phones or browsers, made stricter (or dropped), with the
  reason for each;
- `EXTRA`: bots the list does not have;
- `NAMES`: names for patterns that are not a readable name;
- `FALLBACK`: unknown bots, found by a word such as `examplebot/1.0`.

When several patterns match, the match that starts first in the user agent wins, then the
longest, then the pattern that comes first in the list. `detect(ua)` in `bot-rules.js` is the
reference; the C# detector must give the same for every example of the list and every corpus
user agent.

To update the list, replace `rules/crawler-user-agents.json` with a newer copy, update the
commit in `bot-rules.js` and run `npm run all`. The generator stops if a pattern in `REPLACE`
no longer exists, a pattern needs a name, a regex cannot be converted, or an example of the
list is no longer found. Check that no browser user agent in `expected-results.json` became a
bot (`"bot": null` is expected for browsers).

## How the rules are converted

Every rule is `[regexes, properties]`. `src/rules.js` turns each regex into a
`[GeneratedRegex]` (compiled at build time, with `RegexOptions.ECMAScript` for JavaScript
semantics and the invariant culture), and each property into one of the seven `Assignment`
shapes in `uaParserLibrary/Parsing/Assignment.cs`. A shape, flag or syntax it does not know
stops the generator instead of being converted approximately.

`src/prefilter.js` also works out words that every match of a regex must contain (for
example `"edg"` and `"/"` for Edge), so the parser can skip most regexes with a substring
check. Before writing the rules, the generator checks on every corpus user agent that no
prefilter ever rejects a regex that matches.

## Corpus

| Source | File |
|---|---|
| ua-parser-js 1.0.41's own tests, with their expected values | `corpus/ua-parser-js-1.0.41/*.json` |
| Deliberate changes to those expected values, with reasons | `corpus/upstream-test-differences.json` |
| User agents from the uaParser.Net 1.x tests and samples | `corpus/legacy-port-user-agents.txt` |
| Recent and rare user agents, grouped under `# Heading` lines (also the source of the example user agents) | `corpus/extra-user-agents.txt` |

Expected values are never written by hand: they come from running the rules. When the coverage
report lists a regex that no user agent reaches, add one to `corpus/extra-user-agents.txt`.
