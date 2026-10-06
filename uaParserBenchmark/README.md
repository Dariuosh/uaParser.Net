# uaParserBenchmark

Measures how fast uaParser.Net parses user agents, so you can check it on the machine you
deploy to. It uses the same user agents as `tools/RuleGenerator`:

| Corpus | User agents |
|---|---:|
| `1.x samples` | 722 user agents from the uaParser.Net 1.x tests and samples |
| `recent` | recent and rare user agents |
| `ua-parser-js tests` | the user agents of ua-parser-js 1.0.41's own tests |

## Quick run (about half a minute)

```
cd uaParserBenchmark
dotnet run -c Release -- --quick
```

It prints the first-call (cold start) time and, per corpus, the time to parse one user
agent after warm-up (minimum and median of 15 trials).

To compare with ua-parser-js on the same machine (needs Node.js), run the same measurement
there:

```
cd tools/RuleGenerator
npm ci --ignore-scripts
npm run bench
```

## Full run (BenchmarkDotNet)

```
cd uaParserBenchmark
dotnet run -c Release -- --filter *
```

| Benchmark | What it measures |
|---|---|
| `ParseBenchmarks` | Steady-state time and memory per user agent for `GetClientInfo`, `GetBrowser`, `GetOS` and `GetDevice`, for each corpus |
| `ColdStartBenchmarks` | The first `GetClientInfo` call in a new process (10 processes) |

Run one class with `--filter *ParseBenchmarks*` or `--filter *ColdStart*`.

## Result files

Every run saves its results in `benchmark-results/` at the repository root (ignored by git),
or in the folder given with `--out <folder>`:

| Run | Files |
|---|---|
| `--quick` | `uaParser.Net-quick-<date>.md` and `.json` |
| `npm run bench` | `ua-parser-js-quick-<date>.md` and `.json` (same layout) |
| BenchmarkDotNet | `BenchmarkDotNet/results/`: Markdown, CSV, HTML and JSON reports |

Each file records the processor, number of CPUs, operating system and runtime, but not the
machine name, so results from different devices can be shared and compared.

## Reading the numbers

- Always use `-c Release` and no debugger attached.
- .NET compiles hot code again, optimised, after it has run for a while (tiered compilation),
  so short timings of a few calls overstate the steady-state cost. Both runs above warm up
  first.
- The first call compiles the parser's code. Publishing your application with ReadyToRun
  (`dotnet publish -c Release -r <rid> -p:PublishReadyToRun=true`) makes it about 2.5 times
  faster.
