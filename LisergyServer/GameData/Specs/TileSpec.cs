﻿using GameData.Specs;
using System;
using System.Runtime.InteropServices;

namespace GameData
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TileSpecId
    {
        public byte Id;

        public static implicit operator byte(TileSpecId d) => d.Id;
        public static implicit operator TileSpecId(byte b) => new TileSpecId() { Id = b };

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    [Serializable]
    public class TileSpec
    {
        public TileSpecId ID;
        public ArtSpec TilePrefab;

        /// <summary>
        /// Any resources that are always present on this tile id
        /// </summary>
        public HarvestPointSpecId? ResourceSpotSpecId;

        /// <summary>
        /// Change to tile id when resource is depleted
        /// </summary>
        public TileSpecId ChangeToTileIdWhenDepleted;

        // 1=passable, 0=impassable, 0.5% slower
        public float MovementFactor;

        public TileSpec(in byte i)
        {
            this.ID = new TileSpecId() { Id = i };
            this.TilePrefab = default;
            this.MovementFactor = 1.0f;
        }
    }
}
