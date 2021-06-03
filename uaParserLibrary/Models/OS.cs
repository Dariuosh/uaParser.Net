namespace uaParserLibrary.Models
{
    public sealed class OS
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public OS Empty
        {
            get
            {
                Name = "Other";
                Version = string.Empty;
                return this;
            }
        }

        public override string ToString() => $"{"OS",-7}: {Name} {Version}";
    }
}