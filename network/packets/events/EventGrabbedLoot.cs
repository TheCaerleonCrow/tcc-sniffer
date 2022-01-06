namespace TCC.Sniffer.Templates
{
    /// <summary>
    /// Called when another player loots items.
    /// Is also called when looting silver.
    /// 0:short: ???
    /// 1:string:Victim's Name
    /// 2:string:Looter's Name
    /// 3:bool:isSilver
    /// 4:int:Item Code
    /// 5:long: Amount (If item, the stack amount. If silver, the silver amount.)
    /// </summary>
    public class EventGrabbedLoot : PacketTemplate
    {
        public override PacketType Type => PacketType.EVENT;
        public override short Code => (short)EventCode.OtherGrabbedLoot; // 256

        public string Victim { get; }
        public string Looter { get; }
        public bool IsSilver { get; }
        public int Item { get; }
        public long Amount { get; }

        public EventGrabbedLoot(Dictionary<byte, object> rawData = null) : base(rawData)
        {
            if (rawData == null) return;

            if (rawData.ContainsKey(1))
                Victim = (string)rawData[1];

            if (rawData.ContainsKey(2))
                Looter = (string)rawData[2];

            if (rawData.ContainsKey(3))
                IsSilver = PacketParser.ParseBool(rawData[3]);

            if (rawData.ContainsKey(4))
                Item = PacketParser.ParseInt(rawData[4]);

            if (rawData.ContainsKey(5))
                Amount = PacketParser.ParseLong(rawData[5]);
        }
    }
}
