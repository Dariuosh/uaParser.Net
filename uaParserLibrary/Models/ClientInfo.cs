namespace uaParserLibrary.Models
{
    public  class ClientInfo
    {
        public Browser Browser { get; internal set; }
        public CPU CPU { get; internal set; }
        public Engine Engine { get; internal set; }
        public OS OS { get; internal set; }
        public Device Device { get; internal set; }
    }
}