using System;

namespace Game.Events.ServerEvents
{
    public enum PopupType
    {
        BAD_INPUT = 1,
    }

    [Serializable]
    public class MessagePopupPacket : ServerEvent
    {
        public MessagePopupPacket(PopupType type, params string [] args)
        {
            Type = type;
            Args = args;
        }

        public string[] Args;
        public PopupType Type;
    }
}
