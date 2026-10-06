using System;
using System.Diagnostics;
using System.Linq;

using uaParserLibrary;
using uaParserLibrary.Models;

using uaParserSamples;

namespace uaParserConsole
{
    internal static class Program
    {
        // Parses the user agents given on the command line, or the example user agents, by group.
        private static void Main(string[] args)
        {
            Console.WriteLine($"uaParser.Net (rules {UAParser.RulesVersion}, based on {UAParser.RulesBasedOn})");

            if (args.Length > 0)
            {
                Console.WriteLine();
                foreach (var userAgent in args)
                    Print(userAgent);
                return;
            }

            foreach (var group in SampleUserAgents.All.GroupBy(s => s.Group))
            {
                Console.WriteLine();
                Console.WriteLine($"== {group.Key} ==");
                Console.WriteLine();
                foreach (var sample in group)
                    Print(sample.UserAgent);
            }
        }

        private static void Print(string userAgent)
        {
            var stopwatch = Stopwatch.StartNew();
            ClientInfo info = UAParser.GetClientInfo(userAgent);
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
