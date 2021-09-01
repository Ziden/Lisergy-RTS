using GameData;
using System;
using System.Linq;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class PlayerPacket : ServerEvent
    {
        public Unit[] Units;

        public PlayerPacket(PlayerEntity player)
        {
            Units = player.Units.ToArray();
        }
    }
}
