using Game.Persistence;
using Game.Systems.Battler;
using Game.Systems.Tile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.World
{
    /// <summary>
    /// Flat structure of a 8x8 chunk generated using <see cref="ChunkStructGenerator"/>
    /// This is done this way so we can reference all the chunk memory via single pointer so one memory allocation per chunk
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct ChunkData
    {
        /// <summary>
        /// Tiles can only be accessed via pointer in <see cref="ChunkDataHolder"/>
        /// </summary>
        private TileData _tile_0_0;
        private TileData _tile_0_1;
        private TileData _tile_0_2;
        private TileData _tile_0_3;
        private TileData _tile_0_4;
        private TileData _tile_0_5;
        private TileData _tile_0_6;
        private TileData _tile_0_7;
        private TileData _tile_1_0;
        private TileData _tile_1_1;
        private TileData _tile_1_2;
        private TileData _tile_1_3;
        private TileData _tile_1_4;
        private TileData _tile_1_5;
        private TileData _tile_1_6;
        private TileData _tile_1_7;
        private TileData _tile_2_0;
        private TileData _tile_2_1;
        private TileData _tile_2_2;
        private TileData _tile_2_3;
        private TileData _tile_2_4;
        private TileData _tile_2_5;
        private TileData _tile_2_6;
        private TileData _tile_2_7;
        private TileData _tile_3_0;
        private TileData _tile_3_1;
        private TileData _tile_3_2;
        private TileData _tile_3_3;
        private TileData _tile_3_4;
        private TileData _tile_3_5;
        private TileData _tile_3_6;
        private TileData _tile_3_7;
        private TileData _tile_4_0;
        private TileData _tile_4_1;
        private TileData _tile_4_2;
        private TileData _tile_4_3;
        private TileData _tile_4_4;
        private TileData _tile_4_5;
        private TileData _tile_4_6;
        private TileData _tile_4_7;
        private TileData _tile_5_0;
        private TileData _tile_5_1;
        private TileData _tile_5_2;
        private TileData _tile_5_3;
        private TileData _tile_5_4;
        private TileData _tile_5_5;
        private TileData _tile_5_6;
        private TileData _tile_5_7;
        private TileData _tile_6_0;
        private TileData _tile_6_1;
        private TileData _tile_6_2;
        private TileData _tile_6_3;
        private TileData _tile_6_4;
        private TileData _tile_6_5;
        private TileData _tile_6_6;
        private TileData _tile_6_7;
        private TileData _tile_7_0;
        private TileData _tile_7_1;
        private TileData _tile_7_2;
        private TileData _tile_7_3;
        private TileData _tile_7_4;
        private TileData _tile_7_5;
        private TileData _tile_7_6;
        private TileData _tile_7_7;

        /// <summary>
        /// Chunk data after all tile data for simpler pointer acessing
        /// </summary>
        public byte Flags;
        public TileVector Position;
    }
}
