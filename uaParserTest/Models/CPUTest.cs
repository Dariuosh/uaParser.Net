using uaParserLibrary.Models;

namespace uaParserTest.Models
{
    public class CPUTest
    {
        public string desc { get; set; }
        public string ua { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string architecture { get; set; }
        }

        public bool Validate(CPU cpu)
        {
            return cpu is null ? false : expect.architecture == cpu.Architecture;
        }

        public override string ToString() =>
            $"{desc}\r\n{{Architecture: \"{expect.architecture}\"}}\r\nUA: \"{ua}\"";
    }
}