using Code.World.Noise;
using Game;
using Game.Pathfinder;
using Game.Tile;
using System;
using System.Collections.Generic;

namespace Assets.Code.World
{
    /// <summary>
    /// Big divisions of tiles so we can generate a steady noisemap
    /// </summary>
    public class SectorGeneration
    {
        private static readonly int SECTOR_SHIFT = 7;

        private static Dictionary<Position, float[,]> _noiseMaps = new Dictionary<Position, float[,]>();

        public static float GetTileVertexHeight(int tileX, int tileY, byte vertexX, byte vertexY)
        {
            var sector = GetSector(tileX, tileY);
            if(!_noiseMaps.TryGetValue(sector, out var noise))
            {
                Log.Debug("Generating noise for sector " + sector);
                noise = GenerateSectorHeightmap(sector);
            }
            var sectorMult = new Position(sector.X * 1 << SECTOR_SHIFT, sector.Y * 1 << SECTOR_SHIFT);
            return noise[tileX - sectorMult.X, tileY - sectorMult.Y];
        }

        private static float [,] GenerateSectorHeightmap(Position sector)
        {
            NoiseGen.Seed = sector.GetHashCode();
            var noise = NoiseGen.Calc2D((1 << SECTOR_SHIFT), (1 << SECTOR_SHIFT), 0.1f);
            _noiseMaps[sector] = noise;
            return noise;
        }

        /// <summary>
        /// 128x128 tiles are each sector
        /// </summary>
        public static Position GetSector(int tileX, int tileY)
        {
            return new Position(tileX >> SECTOR_SHIFT, tileY >> SECTOR_SHIFT);
        }
    }
}
