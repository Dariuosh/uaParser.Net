

namespace uaParserLibrary.Models
{
    public sealed class CPU
    {
        public string Architecture { get; set; }


        public override string ToString() => $"{"CPU",-7}: {Architecture}";

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
