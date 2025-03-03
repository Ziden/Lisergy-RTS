using Game.Engine.ECLS;
using GameData;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Tile
{
    /// <summary>
    /// Main struct data used for tiles.
    /// This struct represents a single tile. It's size should be as low as possible.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public class TileSpecComponent : IComponent
    {
        public TileSpecId TileId;

        public override string ToString() => $"<TileData ID={TileId}>";
    }
}
