using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Data
{
    [Serializable]
    public struct TileData
    {
        public byte ResourceId;
        public byte TileId;
        public ushort X;
        public ushort Y;
    }

}
