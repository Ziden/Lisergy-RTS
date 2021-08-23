using Game.Battle;
using Game.Battles;
using Game.Battles.Actions;
using System;

namespace Game.Events
{
    public enum UpdateType
    {
        LOAD = 0,
        ADD = 1,
        REMOVE = 2
    }


    [Serializable]
    public class PlayerDataUpdatePacket : ServerEvent
    {
        public UpdateType Type;
        public BattleHeader[] Battles;
        public Tile[] VisibleTiles;
    }

}
