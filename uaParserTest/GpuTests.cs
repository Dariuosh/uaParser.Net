using uaParserLibrary;
using uaParserLibrary.Models;

using Xunit;

namespace uaParserTest;

// WebGL renderer strings (UNMASKED_RENDERER_WEBGL).
public class GpuTests
{
    [Theory]
    [InlineData("Intel HD Graphics 4000 OpenGL Engine", "Intel", "HD Graphics 4000")]
    [InlineData("Intel Iris Pro OpenGL Engine", "Intel", "Iris Pro")]
    [InlineData("Intel GMA X3100 OpenGL Engine", "Intel", "GMA X3100")]
    [InlineData("ATI Radeon HD 6750M OpenGL Engine", "ATI", "Radeon HD 6750M")]
    [InlineData("NVIDIA GeForce GT 650M OpenGL Engine", "NVIDIA", "GeForce GT 650M")]
    [InlineData("Adreno (TM) 320", "Qualcomm", "Adreno 320")]
    public void Reads_vendor_and_model(string renderer, string vendor, string model)
    {
        Assert.Equal(new GPU(vendor, model), UAParser.GetGPU(renderer));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Apple GPU")]
    public void Unknown_renderer_gives_null_values(string? renderer)
    {
        Assert.Equal(new GPU(null, null), UAParser.GetGPU(renderer));
    }

    [Fact]
    public void Client_info_includes_the_gpu_when_a_renderer_is_given()
    {
        var info = UAParser.GetClientInfo("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)", "Intel Iris Pro OpenGL Engine");

        Assert.Equal(new GPU("Intel", "Iris Pro"), info.GPU);
    }
}
