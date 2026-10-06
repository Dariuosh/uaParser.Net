namespace uaParserDemoComponents.Shared;

/// <summary>One row of a breakdown: a value (null when the user agent has none) and how often it occurs.</summary>
public sealed record Slice(string? Label, long Count);
