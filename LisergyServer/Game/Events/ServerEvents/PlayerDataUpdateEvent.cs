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
    public class PlayerDataUpdateEvent : ServerEvent
    {
        public UpdateType Type;
        public BattleJournalHeader[] Battles;
        public Tile[] VisibleTiles;
    }

}
