namespace uaParserLibrary.Models
{
    public sealed class Engine
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public Engine Empty
        {
            get
            {
                Name = "Other";
                Version = string.Empty;
                return this;
            }
        }

        public override string ToString() => $"{{Name: \"{Name}\", Version: \"{Version}\"}}";
    }
}