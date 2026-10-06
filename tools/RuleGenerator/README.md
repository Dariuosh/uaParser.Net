# RuleGenerator

Builds uaParser.Net's parsing data from [ua-parser-js](https://github.com/faisalman/ua-parser-js)
instead of porting its rules by hand. It loads the real JavaScript library in Node.js and reads
its rule table at run time, so nothing is guessed from the source text.

Only the **1.0.x line (MIT)** is used. ua-parser-js 2.x is licensed under AGPL-3.0; do not copy
rules, code or test data from it.

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
integrity hash of ua-parser-js are pinned in `package-lock.json`. (In October 2021 three
published versions of ua-parser-js, 0.7.29, 0.8.0 and 1.0.0, were hijacked and contained malware.)

| Command | Output |
|---|---|
| `npm run rules` | `uaParserLibrary/Rules/UserAgentRules.g.cs`: the rules as C# |
| `npm run golden` | `uaParserTest/TestData/ua-parser-js.golden.json`: what ua-parser-js returns for every user agent in the corpus (the answer key) |
| `npm run coverage` | `reports/coverage.md`: which upstream regexes the corpus reaches |
| `npm run all` | All three |

The output is deterministic: running it twice gives identical files.

## How the rules are converted

Every upstream rule is `[regexes, properties]`. `src/rules.js` turns each regex into a
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
| ua-parser-js's own tests, with their expected values | `corpus/ua-parser-js-1.0.41/*.json` |
| User agents from the uaParser.Net 1.x tests and samples | `corpus/legacy-port-user-agents.txt` |
| Recent and rare user agents | `corpus/extra-user-agents.txt` |

Expected values are never written by hand: the answer key comes from running ua-parser-js.
Before writing it, the tool checks that ua-parser-js passes all of its own tests.

When the coverage report lists a regex that no user agent reaches, add one to
`corpus/extra-user-agents.txt`.

## Updating to a newer ua-parser-js 1.0.x

1. Set the new version in `package.json` and run `npm install --ignore-scripts`.
2. Copy that tag's `test/*-test.json` and `license.md` into a new `corpus/ua-parser-js-<version>/`
   folder and point `UPSTREAM_DIR` in `src/corpus.js` at it.
3. Run `npm run all` and review the changes to the answer key.
