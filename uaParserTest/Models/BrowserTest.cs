using uaParserLibrary.Models;

namespace uaParserTest.Models
{
    public class BrowserTest
    {
        public string desc { get; set; }
        public string ua { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string name { get; set; }
            public string version { get; set; }
            public string major { get; set; }
        }

        public bool Validate(Browser browser)
        {
            return browser == null
                ? false
                : expect.name == browser.Name
                 && expect.version == browser.Version
                 && expect.major == browser.Major;
        }

        public override string ToString() =>
          $"{desc}\r\n{{Name: \"{expect.name}\", Version: \"{expect.version}\", Major: \"{expect.major}\"}}\r\nUA: \"{ua}\"";
    }
}