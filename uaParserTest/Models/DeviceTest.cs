using uaParserLibrary.Models;

namespace uaParserTest.Models
{
    public class DeviceTest
    {
        public string desc { get; set; }
        public string ua { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string vendor { get; set; }
            public string type { get; set; }
            public string model { get; set; }
        }

        public bool Validate(Device device)
        {
            return device == null
                ? false
                : expect.vendor == device.Vendor
                && expect.type == device.Type
                && expect.model == device.Model;
        }

        public override string ToString() =>
            $"{desc}\r\n{{Vendor: \"{expect.vendor}\", Type: \"{expect.type}\", Model: \"{expect.model}\"}}\r\nUA: \"{ua}\"";
    }
}