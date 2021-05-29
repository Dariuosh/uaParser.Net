using uaParserLibrary.Models;

namespace uaParserTest.Models
{
    public class OSTest
    {
        public string desc { get; set; }
        public string ua { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string name { get; set; }
            public string version { get; set; }
        }

        public bool Validate(OS os)
        {
            return os == null
                ? false
                : expect.name == os.Name
                 && expect.version == os.Version;
        }

        public override string ToString() =>
            $"{desc}\r\n{{Name: \"{expect.name}\", Version: \"{expect.version}\"}}\r\nUA: \"{ua}\"";
    }
}