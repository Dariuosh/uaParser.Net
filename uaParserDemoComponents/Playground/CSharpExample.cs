using System.Text;

using uaParserLibrary.Models;

using uaParserDemoComponents.Shared;

namespace uaParserDemoComponents.Playground;

/// <summary>The C# that gives a result, with the actual values as comments.</summary>
public static class CSharpExample
{
    public static CodeBuilder For(ClientInfo info)
    {
        var code = new CodeBuilder();
        code.Add("using", "keyword").Line(" uaParserLibrary;").Line();
        code.Add("var", "keyword").Add(" info = ").Add("UAParser", "type").Add(".GetClientInfo(")
            .Add(Literal(info.UserAgent), "string").Line(");").Line();

        (string Expression, string? Value)[] lines =
        [
            ("info.Browser.Name", info.Browser.Name),
            ("info.Browser.Version", info.Browser.Version),
            ("info.Browser.Major", info.Browser.Major),
            ("info.Engine.Name", info.Engine.Name),
            ("info.Engine.Version", info.Engine.Version),
            ("info.OS.Name", info.OS.Name),
            ("info.OS.Version", info.OS.Version),
            ("info.Device.Vendor", info.Device.Vendor),
            ("info.Device.Model", info.Device.Model),
            ("info.Device.Type", info.Device.Type),
            ("info.CPU.Architecture", info.CPU.Architecture),
        ];

        var width = lines.Max(l => l.Expression.Length) + 2;
        foreach (var (expression, value) in lines)
        {
            code.Add("Console", "type").Add(".WriteLine(").Add(expression).Add(");".PadRight(width - expression.Length + 2))
                .Line(value is null ? "// null" : $"// {value}", "comment");
        }

        return code;
    }

    // A C# string literal.
    private static string Literal(string value)
    {
        var text = new StringBuilder("\"");
        foreach (var c in value)
        {
            text.Append(c switch
            {
                '"' => "\\\"",
                '\\' => "\\\\",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                _ when char.IsControl(c) => $"\\u{(int)c:x4}",
                _ => c.ToString(),
            });
        }

        return text.Append('"').ToString();
    }
}
