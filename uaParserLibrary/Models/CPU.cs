

namespace uaParserLibrary.Models
{
    public sealed class CPU
    {
        public string Architecture { get; set; }


        public override string ToString() => $"{{Architecture: \"{Architecture}\"}}";

        public CPU Empty
        {
            get
            {
                Architecture = "Other";
                return this;
            }
        }

    }
}
