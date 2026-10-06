namespace uaParserLibrary.Models;

/// <summary>
/// The graphics card, read from a WebGL renderer string (not from the user agent). In a browser,
/// the renderer is gl.getParameter(gl.getExtension('WEBGL_debug_renderer_info').UNMASKED_RENDERER_WEBGL).
/// </summary>
/// <param name="Vendor">For example "Intel" or "NVIDIA", or <see langword="null"/>.</param>
/// <param name="Model">For example "GeForce GT 650M", or <see langword="null"/>.</param>
public sealed record GPU(string? Vendor, string? Model)
{
    public override string ToString() => Describe.Line("GPU", Vendor, Model);
}
