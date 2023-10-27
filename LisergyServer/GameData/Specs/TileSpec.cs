using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class TileSpec
    {
        public byte ID;
        public ArtSpec Art;
        
        /// <summary>
        /// Any resources that are always present on this tile id
        /// </summary>
        public byte? ResourceSpotSpecId;
        
        /// <summary>
        /// Change to tile id when resource is depleted
        /// </summary>
        public byte ChangeToTileIdWhenDepleted;
        
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
