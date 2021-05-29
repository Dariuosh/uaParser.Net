using System;
using System.IO;
using System.Linq;
using System.Text;

namespace uaParserConsole
{
    internal static class ParseDeviceExpressions
    {        private static StringBuilder stringBuilder = new StringBuilder();
        private static void Main(string[] args)
        {    
            var deviceExpressions = File
                // .ReadAllLines("StaticFile/device-expressions.js")
                .ReadAllText("StaticFile/device-expressions.js")
                .Split(new[] { Environment.NewLine, "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries) // split By Lines
                .Select(line => line.Trim())
                //.Where(line =>
                //    !(line.StartsWith("//")
                //     || line.StartsWith("[")
                //     || line.StartsWith("]")
                //    ))
                .Select(line => ParseLines(line))
                ;





            stringBuilder.Clear();
            foreach (var line in deviceExpressions)
            {
                stringBuilder.AppendLine(line);
                Console.WriteLine(line);
            }
            var str = stringBuilder.ToString();
            Console.ReadLine();
        }

     

        public static string ParseLines(string line)
        {
            stringBuilder.Clear();

            var isComment = line.StartsWith("//");
            if (isComment)
            {

                stringBuilder.Append($"\t\t{ line}");
                return stringBuilder.ToString();
            }

            var isCloseBraket = line.StartsWith("],");
            if (isCloseBraket)
            {
                stringBuilder.AppendLine(closeList());
                stringBuilder.Append(createNewList());
                return stringBuilder.ToString();
            }

            var descIndex = line.LastIndexOf("//");

       

            if (descIndex != -1) stringBuilder.AppendLine($"\t\t{line[descIndex..]}");
            stringBuilder.Append(createNewRegex(line[1..line.IndexOf("/i")]));

            return stringBuilder.ToString();
        }

        private static string createNewRegex(string pattern)
        {
            return $"                    Util.CreateRegex(@\"{pattern}\"),";
        }

        private static string createNewList()
        {
            return $@"            new BrowserMatch{{                
                Regexes = new List<Regex>{{";
        }

        private static string closeList()
        {
            return $@"                }},
                Action = SetDevice
            }},";
        }
    }
}