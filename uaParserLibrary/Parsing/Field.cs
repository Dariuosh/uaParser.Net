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
