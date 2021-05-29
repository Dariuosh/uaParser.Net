using uaParserLibrary.Models;

namespace uaParserConsole.Models
{
    public class EngineTest
    {
        public string desc { get; set; }
        public string ua { get; set; }
        public Expect expect { get; set; }

        public class Expect
        {
            public string name { get; set; }
            public string version { get; set; }
        }



        public bool Validate(Engine engine)
        {
            return engine == null
                ? false
                : expect.name == engine.Name
                 && expect.version == engine.Version;
                  
        }
        public override string ToString() =>
            $"{desc}\r\n{{Name: \"{expect.name}\", Version: \"{expect.version}\"}}\r\nUA: \"{ua}\"";
    }
}