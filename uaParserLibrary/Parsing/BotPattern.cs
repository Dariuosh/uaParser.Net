using System.Text.RegularExpressions;

namespace uaParserLibrary.Parsing;

// A bot pattern that is not plain text (see Rules/BotRules.g.cs). Bot: its line in the table.
// Needs: lowercase words every match contains: for each group, at least one of its words.
internal sealed record BotPattern(int Bot, Func<Regex> Regex, string[][] Needs);
