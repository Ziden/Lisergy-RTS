using System;

namespace Game.Events.ServerEvents
{
    public enum PopupType
    {
        INVALID_COURSE = 1
    }

    [Serializable]
    public class MessagePopupEvent : ServerEvent
    {
        public override EventID GetID() => EventID.MESSAGE;

        public MessagePopupEvent(PopupType type, params string [] args)
        {
            Type = type;
            Args = args;
        }

        public string[] Args;
        public PopupType Type;
    }
}
