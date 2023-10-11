using Game.Network;
using Game.Systems.Tile;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TilePacket : BasePacket, IServerPacket
    {
        public TilePacket(in TileMapData data)
        {
            _data = data;
        }

        private TileMapData _data;

        public ref readonly TileMapData Data => ref _data;

        public override string ToString()
        {
            return $"<TilePacket {_data}>";
        }
    }
}
