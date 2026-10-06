namespace uaParserLibrary.Models;

/// <summary>The device the user agent comes from.</summary>
/// <param name="Vendor">For example "Apple" or "Samsung", or <see langword="null"/>.</param>
/// <param name="Model">For example "iPhone" or "SM-S931B", or <see langword="null"/>.</param>
/// <param name="Type">One of the <see cref="DeviceTypes"/> values, or <see langword="null"/> (usually a desktop).</param>
public sealed record Device(string? Vendor, string? Model, string? Type)
{
    public override string ToString() => Describe.Line("Device", Vendor, Model, Type);
}
