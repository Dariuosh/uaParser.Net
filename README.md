<p align="center">
  <a href="https://www.nuget.org/packages/uaParser.Net">
    <img src="https://img.shields.io/nuget/v/uaParser.Net.svg?style=flat-square">
  </a>
</p>

# uaParser.cs

.Net library to detect Browser, Engine, OS, CPU, and Device type/model from User-Agent data Based on https://github.com/faisalman/ua-parser-js which can be used in the .Net Core platform or as a middleware in ASP.Net Core

# Documentation

## Methods (uaParserLibrary)

- `UAParser.GetBrowser(string)`
  - returns `Browser`

- `UAParser.GetDevice(string)`
  - returns `Device`

- `UAParser.GetEngine(string)`
  - returns `Engine`

- `UAParser.GetOS(string)`
  - returns `OS`

- `UAParser.GetCPU(string)`
  - returns `CPU`

- `UAParser.GetGPU(string)`
  - returns `GPU`

- `UAParser.GetClientInfo(string)`
- `UAParser.GetClientInfo(string,string)`
  - returns `ClientInfo`

# License

MIT License
