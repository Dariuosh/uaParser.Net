namespace uaParserLibrary.Models
{
    public sealed class GPU
    {
        public string Model { get; set; }
        public string Vendor { get; set; }

        public GPU Empty
        {
            get
            {
                Model = "Other";
                Vendor = "Other";
                return this;
            }
        }

        public override string ToString() => $"{{Vendor: \"{Vendor}\", Model: \"{Model}\"}}";
    }
}

/// -----------
///
/// -----------

//var renderer;
//// browser only
//if (window && window.document)
//{
//    var canvas = document.createElement('canvas');
//    var gl = canvas.getContext ? canvas.getContext('webgl2') || canvas.getContext('webgl') || canvas.getContext('experimental-webgl') : undefined;
//    renderer = gl && gl.getParameter && gl.getExtension && gl.getExtension('WEBGL_debug_renderer_info') ? gl.getParameter(gl.getExtension('WEBGL_debug_renderer_info').UNMASKED_RENDERER_WEBGL) : undefined;
//}