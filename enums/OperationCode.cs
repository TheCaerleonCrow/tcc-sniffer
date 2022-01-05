namespace TCC.Sniffer
{
    public enum OperationCode
    {
        NONE = -1,
        ALL = 0,

        Join = 2,
        Move = 22,
        LogoutStart = 122,
        GetCharacterStats = 139,

        GoldBuyOrderPrice = 237,
        GoldSellOrderPrice = 239,
        BuyGold = 241,
        SellGold = 242,
        GoldStats = 245,
        GoldHistory = 247,

        ClientHardwareStats = 299,
        ClientPerformanceStats = 300,
    }
}
