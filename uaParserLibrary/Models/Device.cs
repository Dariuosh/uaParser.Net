namespace uaParserLibrary.Models
{
    public sealed class Device
    {
        public string Vendor { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }

        public Device Empty
        {
            get
            {
                Vendor = "UnKnown";
                Type = string.Empty;
                Model =string.Empty;
                return this;
            }
        }

        public override string ToString() => $"{"Device",-7}: {Vendor} {Type} {Model}";
    }
}