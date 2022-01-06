namespace TCC.Sniffer.Templates
{
    /// <summary>
    /// Called when silver or gold is updated.
    /// 0:????
    /// 1:long:Silver
    /// 2:int:Gold
    /// </summary>
    public class EventUpdateMoney : PacketTemplate
    {
        public override PacketType Type => PacketType.EVENT;
        public override short Code => (short)EventCode.UpdateMoney; // 71

        public long Silver { get; }
        public long Gold { get; }

        public EventUpdateMoney(Dictionary<byte, object> rawData = null) : base(rawData)
        {
            if (rawData == null) return;

            if (rawData.ContainsKey(1))
                Silver = PacketParser.ParseLong(rawData[1]);

            if (rawData.ContainsKey(2))
                Gold = PacketParser.ParseLong(rawData[2]);
        }
    }
}
