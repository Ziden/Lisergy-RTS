using Game.Systems.Tile;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class TileUpdatePacket : ServerPacket
    {
        public TileUpdatePacket(in TileData data)
        {
            _data = data;
        }

        private TileData _data;

        public ref TileData Data => ref _data;

        public override string ToString()
        {
            return $"<TileUpdate {_data}>";
        }
    }
}
