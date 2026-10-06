using System.Text;
using System.Text.Json;

namespace uaParserDemoComponents.AccessLogs;

/// <summary>
/// Finds the user agent in a line of a web server log. Reads, line by line:
/// <list type="bullet">
/// <item>Apache and nginx "combined" logs (and nginx's "main"): the third quoted field;</item>
/// <item>IIS W3C logs: the cs(User-Agent) column named by the #Fields line, with + for spaces;</item>
/// <item>JSON lines (Caddy, Traefik, ...): a property named User-Agent, user_agent or similar;</item>
/// <item>anything else: the whole line is one user agent.</item>
/// </list>
/// </summary>
public sealed class LogLineReader
{
    private int _iisColumn = -1;

    /// <summary>The kind of log the last line read looked like, for display; null for blank and comment lines.</summary>
    public string? Format { get; private set; }

    /// <summary>The user agent of the line, or null when it has none ("-", a comment, a blank line).</summary>
    public string? Read(string line)
    {
        Format = null;
        var text = line.AsSpan().Trim();
        if (text.IsEmpty)
            return null;

        if (text[0] == '#')
        {
            ReadDirective(text.ToString());
            return null;
        }

        if (_iisColumn >= 0)
            return ReadIis(text);

        if (text[0] == '{')
            return ReadJson(text.ToString());

        if (text.Contains('"'))
            return ReadQuoted(text);

        Format = "one user agent per line";
        return Clean(text.ToString());
    }

    // IIS: "#Fields: date time ... cs(User-Agent) ..." names the columns of the lines that follow.
    private void ReadDirective(string line)
    {
        const string fields = "#Fields:";
        if (!line.StartsWith(fields, StringComparison.OrdinalIgnoreCase))
            return;

        var names = line[fields.Length..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        _iisColumn = Array.FindIndex(names, n => n.Equals("cs(User-Agent)", StringComparison.OrdinalIgnoreCase));
    }

    private string? ReadIis(ReadOnlySpan<char> line)
    {
        Format = "IIS (W3C)";
        var column = 0;
        foreach (var range in line.Split(' '))
        {
            if (column++ == _iisColumn)
                return Clean(line[range].ToString().Replace('+', ' '));
        }

        return null;
    }

    private string? ReadQuoted(ReadOnlySpan<char> line)
    {
        // Apache: %h %l %u %t "%r" %>s %b "%{Referer}i" "%{User-agent}i"; nginx adds more quoted fields after it.
        var quoted = QuotedFields(line);
        if (quoted.Count >= 3)
        {
            Format = "Apache / nginx (combined)";
            return Clean(quoted[2]);
        }

        // A quoted user agent on its own, as in a one-column CSV file.
        if (quoted.Count == 1 && line[0] == '"' && line[^1] == '"')
        {
            Format = "one user agent per line";
            return Clean(quoted[0]);
        }

        // The common log format has no user agent.
        Format = "a log without user agents";
        return null;
    }

    private static List<string> QuotedFields(ReadOnlySpan<char> line)
    {
        var fields = new List<string>(4);
        var field = new StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] != '"')
                continue;

            field.Clear();
            for (i++; i < line.Length && line[i] != '"'; i++)
            {
                // Apache writes \" and \\; nginx writes \x22.
                if (line[i] == '\\' && i + 1 < line.Length)
                {
                    if (line[i + 1] is '"' or '\\')
                    {
                        field.Append(line[++i]);
                        continue;
                    }

                    if (line[i + 1] == 'x' && i + 3 < line.Length && byte.TryParse(line.Slice(i + 2, 2), System.Globalization.NumberStyles.HexNumber, null, out var code))
                    {
                        field.Append((char)code);
                        i += 3;
                        continue;
                    }
                }

                field.Append(line[i]);
            }

            fields.Add(field.ToString());
        }

        return fields;
    }

    private string? ReadJson(string line)
    {
        Format = "JSON lines";
        try
        {
            using var document = JsonDocument.Parse(line);
            return Clean(Find(document.RootElement, depth: 0));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? Find(JsonElement element, int depth)
    {
        if (depth > 8)
            return null;

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (IsUserAgentName(property.Name) && FirstString(property.Value) is { } value)
                    return value;
            }

            foreach (var property in element.EnumerateObject())
            {
                if (Find(property.Value, depth + 1) is { } nested)
                    return nested;
            }
        }

        return null;
    }

    private static bool IsUserAgentName(string name)
    {
        Span<char> letters = stackalloc char[Math.Min(name.Length, 64)];
        var count = 0;
        foreach (var c in name)
        {
            if (char.IsLetter(c) && count < letters.Length)
                letters[count++] = char.ToLowerInvariant(c);
        }

        // user-agent, user_agent, userAgent, http_user_agent, request_User-Agent, ...
        return letters[..count].EndsWith("useragent");
    }

    // Caddy logs headers as arrays: "User-Agent": ["..."].
    private static string? FirstString(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString(),
        JsonValueKind.Array => value.EnumerateArray().Where(v => v.ValueKind == JsonValueKind.String).Select(v => v.GetString()).FirstOrDefault(),
        _ => null,
    };

    private static string? Clean(string? userAgent) =>
        string.IsNullOrWhiteSpace(userAgent) || userAgent.Trim() == "-" ? null : userAgent.Trim();
}
