namespace uaParserLibrary.Models;

internal static class Describe
{
    // "Browser: Chrome 140.0.0.0": the label, then the values that are present.
    public static string Line(string label, params string?[] values) =>
        $"{label,-7}: {string.Join(' ', values.Where(v => !string.IsNullOrEmpty(v)))}";
}
