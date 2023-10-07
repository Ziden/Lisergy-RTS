using Game.Network;
using Game.Systems.Tile;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TilePacket : ServerPacket
    {
        public TilePacket(in TileMapData data)
        {
            _data = data;
        }

        private TileMapData _data;

        public ref TileMapData Data => ref _data;

        public override string ToString()
        {
            return $"<TilePacket {_data}>";
        }
    }
}
