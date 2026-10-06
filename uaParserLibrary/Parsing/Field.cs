namespace uaParserLibrary.Parsing;

// The values a rule can set. The models pick the ones they need.
internal enum Field
{
    Name,
    Version,
    Major,
    Architecture,
    Vendor,
    Model,
    Type,
}

internal static class Fields
{
    public const int Count = 7;
}

// The values one rule set produces, indexed by Field. A struct, so parsing does not allocate
// an array for it.
[System.Runtime.CompilerServices.InlineArray(Fields.Count)]
internal struct FieldValues
{
    private string? _first;

    public string? this[Field field]
    {
        readonly get => this[(int)field];
        set => this[(int)field] = value;
    }
}
