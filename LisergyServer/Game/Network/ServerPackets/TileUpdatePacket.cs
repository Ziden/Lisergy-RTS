using Game.Comands;
using Game.ECS;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileUpdatePacket : BasePacket, IServerPacket
    {
        public TileData Data;
        public TileVector Position;
        public List<IComponent> Components; 

        public override string ToString()
        {
            return $"<TilePacket {Position} {Data} Components={Components?.Count}>";
        }
    }
}
