using uaParserLibrary.Models;

namespace uaParserConsole.Models
{
    public class GPUTest
    {
        public string desc { get; set; }
        public string renderer { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string vendor { get; set; }
            public string model { get; set; }
        }

        public bool Validate(GPU gpu)
        {
            return gpu == null
                ? false
                : expect.model == gpu.Model
                 && expect.vendor == gpu.Vendor;
        }

        public override string ToString() =>
            $"{desc}\r\n{{Vendor: \"{expect.vendor}\", Model: \"{expect.model}\"}}\r\nUA: \"{renderer}\"";
    }
}