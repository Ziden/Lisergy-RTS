using Game.Systems.Tile;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileUpdatePacket : ServerPacket
    {
        public TileUpdatePacket(in TileMapData data)
        {
            _data = data;
        }

        private TileMapData _data;

        public ref TileMapData Data => ref _data;

        public override string ToString()
        {
            return $"<TileUpdate {_data}>";
        }
    }
}
