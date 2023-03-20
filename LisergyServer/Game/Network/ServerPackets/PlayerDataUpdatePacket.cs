using Game.Battle;
using Game.Tile;
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
    public class PlayerUpdatePacket : ServerPacket
    {
        public UpdateType Type;
        public BattleHeader[] Battles;
        public TileEntity[] VisibleTiles;
    }

}
