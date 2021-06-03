using System.Text.RegularExpressions;

using uaParserLibrary.Expressions;
using uaParserLibrary.Models;

namespace uaParserLibrary
{
    public static class UAParser
    {
        private static Browser browser { get; set; }

        private static Regex regex = Util.CreateRegex(@"\d*");

        public static Browser GetBrowser(string UserAgent)
        {
            browser ??= new Browser();

            foreach (var matchItem in BrowserExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(UserAgent))
                    {
                        var match = regexItem.Match(UserAgent);

                        matchItem.Action(match, browser);

                        browser.Major = regex.Match(browser.Version).Value;

                        return browser;
                    }
                }
            }

            return browser.Empty;
        }

        private static CPU cpu { get; set; }

        public static CPU GetCPU(string UserAgent)
        {
            cpu ??= new CPU();

            foreach (var matchItem in CPUExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(UserAgent))
                    {
                        var match = regexItem.Match(UserAgent);
                        matchItem.Action(match, cpu);
                        return cpu;
                    }
                }
            }

            return cpu.Empty;
        }

        private static Engine engine { get; set; }

        public static Engine GetEngine(string UserAgent)
        {
            engine ??= new Engine();

            foreach (var matchItem in EngineExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(UserAgent))
                    {
                        var match = regexItem.Match(UserAgent);

                        matchItem.Action(match, engine);

                        return engine;
                    }
                }
            }

            return engine.Empty;
        }

        private static OS os { get; set; }

        public static OS GetOS(string UserAgent)
        {
            os ??= new OS();

            foreach (var matchItem in OSExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(UserAgent))
                    {
                        var match = regexItem.Match(UserAgent);

                        matchItem.Action(match, os);

                        return os;
                    }
                }
            }

            return os.Empty;
        }

        private static Device device { get; set; }

        public static Device GetDevice(string UserAgent)
        {
            device ??= new Device();

            foreach (var matchItem in DeviceExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(UserAgent))
                    {
                        var match = regexItem.Match(UserAgent);

                        matchItem.Action(match, device);

                        return device;
                    }
                }
            }

            return device.Empty;
        }

        private static GPU gpu { get; set; }

        public static GPU GetGPU(string renderer)
        {
            gpu ??= new GPU();

            foreach (var matchItem in GPUExpressions.Matches)
            {
                foreach (var regexItem in matchItem.Regexes)
                {
                    if (regexItem.IsMatch(renderer))
                    {
                        var match = regexItem.Match(renderer);

                        matchItem.Action(match, gpu);

                        return gpu;
                    }
                }
            }

            return gpu.Empty;
        }

        private static ClientInfo clientInfo { get; set; }

        public static ClientInfo GetClientInfo(string UserAgent)
        {
            clientInfo ??= new ClientInfo();

            clientInfo.Browser = GetBrowser(UserAgent);
            clientInfo.CPU = GetCPU(UserAgent);
            clientInfo.Engine = GetEngine(UserAgent);
            clientInfo.OS = GetOS(UserAgent);
            clientInfo.Device = GetDevice(UserAgent);
            return clientInfo;
        }

        public static ClientInfo GetClientInfo(string UserAgent,string Renderer)
        {
            clientInfo ??= new ClientInfo();

            clientInfo.Browser = GetBrowser(UserAgent);
            clientInfo.CPU = GetCPU(UserAgent);
            clientInfo.Engine = GetEngine(UserAgent);
            clientInfo.OS = GetOS(UserAgent);
            clientInfo.Device = GetDevice(UserAgent);
            clientInfo.GPU = GetGPU(Renderer);
            return clientInfo;
        }




        private static string ReadQuotedValue(string value)
        {
            if (value.StartsWith("'") && value.EndsWith("'") || (value.StartsWith("\"") && value.EndsWith("\"")))
                return value.Substring(1, value.Length - 2);

            return value;
        }
    }
}

//Methods
//  getBrowser()
//      returns { name: '', version: '' }
//  getDevice()
//      returns { model: '', type: '', vendor: '' }
//  getEngine()
//      returns { name: '', version: '' }
//  getOS()
//      returns { name: '', version: '' }
//  getCPU()
//      returns { architecture: '' }
//  getResult()
//      returns { ua: '', browser: { }, cpu: { }, device: { }, engine: { }, os: { } }
//  getUA()
//      returns UA string of current instance
//  setUA(uastring)
//      set UA string to be parsed
//      returns current instance