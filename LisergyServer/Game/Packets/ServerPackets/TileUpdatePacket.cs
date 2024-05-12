using Game.ECS;
using Game.Engine.Network;
using Game.Systems.Tile;
using Game.World;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileUpdatePacket : BasePacket, IServerPacket
    {
        public TileData Data;
        public Location Position;
        public IComponent [] Components; 
        // public List<uint> Removed; // Todo: centralize with entity update packet ?

        public override string ToString()
        {
            return $"<TilePacket {Position} {Data} Components={Components?.Length}>";
        }
    }
}
