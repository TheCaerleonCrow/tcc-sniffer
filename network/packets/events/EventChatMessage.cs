using System;
using System.Collections.Generic;

namespace TCC.Sniffer.Templates
{
    /// <summary>
    /// Called when a chat message is recieved.
    /// 0:int:ChannelCode
    /// 1:string:Player
    /// 2:string:Message
    /// 3:byte: Unknown. Could be some code for normal player / mod / admin. Would need mods/admins to send messages to figure that out.
    /// </summary>
    public class EventChatMessage : PacketTemplate
    {
        public override PacketType Type => PacketType.EVENT;
        public override short Code => (short)EventCode.ChatMessage; // 63

        public ChatChannelCode ChannelCode { get; }
        public string Player { get; }
        public string Message { get; }

        public EventChatMessage(Dictionary<byte, object> rawData = null) : base(rawData)
        {
            if (rawData == null) return;

            ChannelCode = (ChatChannelCode)Convert.ToInt32(rawData[0]);
            Player = (string)rawData[1];
            Message = (string)rawData[2];
        }
    }
}
