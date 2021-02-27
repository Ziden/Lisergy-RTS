using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class TileSpec
    {
        public byte ID;
        public List<ArtSpec> Arts;

        // 1=passable, 0=impassable, 0.5% slower
        public float MovementFactor = 1.0f;
    }
}
