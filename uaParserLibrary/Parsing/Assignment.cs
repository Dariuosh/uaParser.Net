using System.Text.RegularExpressions;

namespace uaParserLibrary.Parsing;

// How one property of a matched rule is set, mirroring the shapes rgxMapper understands.
// "match" is the capture group: null when the group took no part in the match (JavaScript's
// undefined). Where rgxMapper tests `match ? ... : undefined`, an empty match counts as none.
internal sealed class Assignment
{
    private readonly Kind _kind;
    private readonly string? _value;
    private readonly Func<Regex>? _pattern;
    private readonly bool _replaceAll;
    private readonly StringMap? _map;

    private Assignment(Field field, Kind kind, string? value = null, Func<Regex>? pattern = null, bool replaceAll = false, StringMap? map = null)
    {
        Field = field;
        _kind = kind;
        _value = value;
        _pattern = pattern;
        _replaceAll = replaceAll;
        _map = map;
    }

    private enum Kind
    {
        Capture,
        Constant,
        Lowercase,
        TrimStart,
        Replace,
        ReplaceThenLowercase,
        Map,
    }

    public Field Field { get; }

    // PROP
    public static Assignment Capture(Field field) => new(field, Kind.Capture);

    // [PROP, 'value']
    public static Assignment Constant(Field field, string? value) => new(field, Kind.Constant, value);

    // [PROP, lowerize]
    public static Assignment Lowercase(Field field) => new(field, Kind.Lowercase);

    // [PROP, trim]
    public static Assignment TrimStart(Field field) => new(field, Kind.TrimStart);

    // [PROP, /regex/, 'replacement']
    public static Assignment Replace(Field field, Func<Regex> pattern, string replacement, bool all) =>
        new(field, Kind.Replace, replacement, pattern, all);

    // [PROP, /regex/, 'replacement', lowerize]
    public static Assignment ReplaceThenLowercase(Field field, Func<Regex> pattern, string replacement, bool all) =>
        new(field, Kind.ReplaceThenLowercase, replacement, pattern, all);

    // [PROP, strMapper, map]
    public static Assignment Map(Field field, StringMap map) => new(field, Kind.Map, map: map);

    public string? Apply(Group group)
    {
        var match = group.Success ? group.Value : null;
        var hasMatch = !string.IsNullOrEmpty(match);

        return _kind switch
        {
            Kind.Capture => hasMatch ? match : null,
            Kind.Constant => _value,
            Kind.Lowercase => match?.ToLowerInvariant(),
            Kind.TrimStart => match is null ? null : JsString.TrimStart(match),
            Kind.Replace => hasMatch ? Replace(match!) : null,
            Kind.ReplaceThenLowercase => hasMatch ? Replace(match!).ToLowerInvariant() : null,
            Kind.Map => hasMatch ? _map!.Map(match!) : null,
            _ => throw new InvalidOperationException($"Unknown assignment kind {_kind}."),
        };
    }

    // String.prototype.replace: every match for a /g regex, otherwise only the first.
    private string Replace(string input) =>
        _pattern!().Replace(input, _value!, _replaceAll ? -1 : 1);
}
