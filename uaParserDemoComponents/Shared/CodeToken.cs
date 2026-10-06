using System.Text;
using System.Text.RegularExpressions;

namespace uaParserDemoComponents.Shared;

/// <summary>A piece of code and its kind ("keyword", "string", "comment", "number", "member"), or null for plain text.</summary>
public sealed record CodeToken(string Text, string? Kind = null);

/// <summary>Builds colored code: append tokens, then read <see cref="Text"/> and <see cref="Tokens"/>.</summary>
public sealed class CodeBuilder
{
    private readonly List<CodeToken> _tokens = [];
    private readonly StringBuilder _text = new();

    public IReadOnlyList<CodeToken> Tokens => _tokens;

    public string Text => _text.ToString();

    public CodeBuilder Add(string text, string? kind = null)
    {
        if (text.Length == 0)
            return this;

        _tokens.Add(new CodeToken(text, kind));
        _text.Append(text);
        return this;
    }

    public CodeBuilder Line(string text = "", string? kind = null) => Add(text, kind).Add("\n");
}

/// <summary>Colors short C# and Razor snippets: comments, strings, keywords and type names.</summary>
public static partial class CSharpTokens
{
    public static IReadOnlyList<CodeToken> Split(string code)
    {
        var builder = new CodeBuilder();
        var position = 0;

        foreach (Match match in Token().Matches(code))
        {
            builder.Add(code[position..match.Index]);
            var kind = match.Groups["comment"].Success ? "comment"
                : match.Groups["string"].Success ? "string"
                : match.Groups["keyword"].Success ? "keyword"
                : "type";
            builder.Add(match.Value, kind);
            position = match.Index + match.Length;
        }

        builder.Add(code[position..]);
        return builder.Tokens;
    }

    // A type: a capitalized name that is not a member access (no '.' before it).
    [GeneratedRegex("""(?<comment>//[^\n]*)|(?<string>"(?:[^"\\\n]|\\.)*")|(?<keyword>@?\b(?:var|new|using|await|async|return|public|private|static|class|string|int|if|else|null|true|false|inject|code|page)\b)|(?<![.\w])(?<type>[A-Z]\w*)(?=[.<(\s])""")]
    private static partial Regex Token();
}

/// <summary>Colors JSON text.</summary>
public static partial class JsonTokens
{
    public static IReadOnlyList<CodeToken> Split(string json)
    {
        var builder = new CodeBuilder();
        var position = 0;

        foreach (Match match in Token().Matches(json))
        {
            builder.Add(json[position..match.Index]);
            var kind = match.Groups["key"].Success ? "member"
                : match.Groups["string"].Success ? "string"
                : match.Groups["number"].Success ? "number"
                : "keyword";
            builder.Add(match.Value, kind);
            position = match.Index + match.Length;
        }

        builder.Add(json[position..]);
        return builder.Tokens;
    }

    [GeneratedRegex("""(?<key>"(?:[^"\\]|\\.)*"(?=\s*:))|(?<string>"(?:[^"\\]|\\.)*")|(?<number>-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)|(?<literal>\btrue\b|\bfalse\b|\bnull\b)""")]
    private static partial Regex Token();
}
