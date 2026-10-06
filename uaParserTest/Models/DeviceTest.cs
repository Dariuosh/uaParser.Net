using uaParserLibrary.Models;

using uaParserResource;

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
            if (device == null)
                return false;

            // As in ua-parser-js's own tests, a missing expectation means undefined.
            var vendor = expect.vendor ?? Keywords.Undefined;
            var type = expect.type ?? Keywords.Undefined;
            var model = expect.model ?? Keywords.Undefined;

            // Nothing detected: the parser reports its Empty device instead.
            if (vendor == Keywords.Undefined && type == Keywords.Undefined && model == Keywords.Undefined)
            {
                var empty = new Device().Empty;
                return device.Vendor == empty.Vendor
                    && device.Type == empty.Type
                    && device.Model == empty.Model;
            }

            return vendor == device.Vendor
                && type == device.Type
                && model == device.Model;
        }

        public override string ToString() =>
            $"{desc}\r\n{{Vendor: \"{expect.vendor}\", Type: \"{expect.type}\", Model: \"{expect.model}\"}}\r\nUA: \"{ua}\"";
    }
}