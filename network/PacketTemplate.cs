using System.Collections.Generic;

namespace TCC.Sniffer.Templates
{
    public abstract class PacketTemplate
    {
        public abstract PacketType Type { get; }
        public abstract short Code { get; }
        public PacketTemplate(Dictionary<byte, object> rawData = null) { }
    }
}
