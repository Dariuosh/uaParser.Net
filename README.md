<p align="center">
  <a href="https://www.nuget.org/packages/uaParser.Net">
    <img src="https://img.shields.io/nuget/v/uaParser.Net.svg?style=flat-square">
  </a>
</p>

# uaParser.Net

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

## Examples
### Console App
```C#
using System;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserConsole
{
    internal static class Program

    {
        private static void Main(string[] args)
        {
            string uaString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4501.0 Safari/537.36 Edg/92.0.891.0";

            Browser browser = UAParser.GetBrowser(uaString);
            Console.WriteLine(browser);          // Browser: Edge 92.0.891.0 
            Console.WriteLine(browser.Name);     // Edge 

            CPU cpu = UAParser.GetCPU(uaString);
            Console.WriteLine(cpu);              // CPU    : amd64
            Console.WriteLine(cpu.Architecture); // amd64

            Engine engine = UAParser.GetEngine(uaString);
            Console.WriteLine(engine);           // Engine : Blink 92.0.4501.0
            Console.WriteLine(engine.Name);      // Blink

            OS os = UAParser.GetOS(uaString);
            Console.WriteLine(os);               // OS     : Windows 10
            Console.WriteLine(os.Name);          // Windows

            Device device = UAParser.GetDevice(uaString);
            Console.WriteLine(device);           // Device : UnKnown

            ClientInfo clientInfo = UAParser.GetClientInfo(uaString);
            Console.WriteLine(clientInfo.Browser);           // Browser: Edge 92.0.891.0
            Console.WriteLine(clientInfo.CPU.Architecture);  // amd64
            Console.WriteLine(clientInfo.OS);                // OS     : Windows 10
            Console.WriteLine(clientInfo.Engine.Name);       // Blink
        }
    }
}
```

# License

MIT License
