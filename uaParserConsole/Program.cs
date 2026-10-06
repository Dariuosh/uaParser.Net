using System;
using System.Collections.Generic;
using System.Diagnostics;

using uaParserLibrary;

namespace uaParserConsole
{
    internal static class Program
    {
        // Parses the user agents given on the command line, or the sample list when there are none.
        private static void Main(string[] args)
        {
            IReadOnlyList<string> userAgents = args.Length > 0 ? args : UserAgentSampleList.UserAgent;
            var stopwatch = new Stopwatch();

            Console.WriteLine($"uaParser.Net (rules from ua-parser-js {UAParser.RulesVersion})");
            Console.WriteLine();

            foreach (var userAgent in userAgents)
            {
                stopwatch.Restart();
                var info = UAParser.GetClientInfo(userAgent);
                stopwatch.Stop();

                Console.WriteLine(info.UserAgent);
                Console.WriteLine($"  {info.Browser}");
                Console.WriteLine($"  {info.Engine}");
                Console.WriteLine($"  {info.OS}");
                Console.WriteLine($"  {info.Device}");
                Console.WriteLine($"  {info.CPU}");
                Console.WriteLine($"  ({stopwatch.Elapsed.TotalMilliseconds:F3} ms)");
                Console.WriteLine();
            }
        }
    }
}
