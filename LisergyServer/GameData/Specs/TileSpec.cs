using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public struct TileSpec
    {
        public byte ID;
        public ArtSpec Art;

        // 1=passable, 0=impassable, 0.5% slower
        public float MovementFactor;

        public TileSpec(byte i)
        {
            this.ID = i;
            this.Art = default;
            this.MovementFactor = 1.0f;
        }
    }
}
