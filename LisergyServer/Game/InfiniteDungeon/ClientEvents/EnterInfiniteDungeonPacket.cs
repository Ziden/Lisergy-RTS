using System;

namespace Game.Events.ClientEvents
{
    [Serializable]
    public class EnterInfiniteDungeonPacket : ClientEvent
    {
        public string[] UnitIds;
    }
}
