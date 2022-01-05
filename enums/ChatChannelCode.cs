namespace TCC.Sniffer
{
    public enum ChatChannelCode
    {
        Global = 1,
        English = 2,
        German = 3,
        French = 4,
        Polish = 5,
        Russian = 6,
        Spanish = 7,
        Portuguese = 8,
        Chinese = 9,
        Korean = 10,
        Trade = 11,
        Recruit = 12,
        LFG = 13,
        Help = 14,
        Local = 536,

        // These seem to change for some reason... futher comments are other values I've seen them as.
        // Most likely these channels have some range they can be in, which are then shuffled at server reset time.
        // They will also likely need special code to properly parse.
        Faction = 1755, //1761,
        Guild = 2492, //104320, 74883,
    }
}
