namespace TCC.Sniffer.Templates
{
    /// <summary>
    /// Called when another player loots items.
    /// 0:short: ???
    /// 1:string:Victim's Name
    /// 2:string:Looter's Name
    /// 3:bool: ???
    /// 4:int:Item Code
    /// 5:short: ???
    /// </summary>
    public class EventOtherGrabbedLoot : PacketTemplate
    {
        public override PacketType Type => PacketType.EVENT;
        public override short Code => (short)EventCode.OtherGrabbedLoot; // 256

        public string Victim { get; }
        public string Looter { get; }
        public int Item { get; }

        public EventOtherGrabbedLoot(Dictionary<byte, object> rawData = null) : base(rawData)
        {
            if (rawData == null) return;

            Victim = (string)rawData[1];
            Looter = (string)rawData[1];
            Item = PacketParser.ParseInt(rawData[4]);
        }
    }
}
