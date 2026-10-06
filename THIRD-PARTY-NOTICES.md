# Third-party notices

uaParser.Net includes material from the projects below, under their licenses.

## ua-parser-js

https://github.com/faisalman/ua-parser-js

Used from version 1.0.41 (MIT). Nothing is taken from ua-parser-js 2.x, which is licensed under
AGPL-3.0.

- `tools/RuleGenerator/rules/user-agent-rules.js`: the user agent rules, copied from ua-parser-js
  1.0.41 and since then changed and extended by uaParser.Net (changes are marked there).
- `uaParserLibrary/Rules/UserAgentRules.g.cs`: those rules, converted to C# by
  `tools/RuleGenerator`.
- `uaParserLibrary/Rules/GpuRules.cs`: GPU rules from the ua-parser-js development branch of 2021,
  as ported in uaParser.Net 1.x.
- `tools/RuleGenerator/corpus/ua-parser-js-1.0.41/`: ua-parser-js 1.0.41's test data, used by the
  tests.

```
MIT License

Copyright (c) 2012-2025 Faisal Salman <<f@faisalman.com>>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
