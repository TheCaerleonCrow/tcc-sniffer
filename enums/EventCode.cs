namespace TCC.Sniffer
{
    public enum EventCode
    {
        NONE = -1,
        ALL = 0,

        Move = 3,
        NewBuilding = 36,
        HarvestFinished = 52,
        ChatMessage = 63,
        UpdateMoney = 71,
        UpdateFame = 72,
        UpdateRespecPoints = 74,
        GuildUpdate = 92,
        GuildMemberUpdate = 93,
        CharacterStats = 130,
        CharacterKillHistory = 131,
        CharacterDeathHistory = 132,
        DestinyBoardInfo = 135,
        DestinyBoardStats = 138,
        Unknown144 = 144,
        StartLogout = 189,
        InCombatStateUpdate = 254,
    }
}
