using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class GameSpec
    {
        public int Version = 1;

        public byte InitialBuilding;
        public Dictionary<byte, BuildingSpec> Buildings = new Dictionary<byte, BuildingSpec>();
        public Dictionary<int, TileSpec> Tiles = new Dictionary<int, TileSpec>();

        public BuildingSpec GetBuildingSpec(byte id)
        {
            BuildingSpec spec;
            if (!Buildings.TryGetValue(id, out spec))
                throw new Exception($"Building spec {id} does not exists");
            return spec;
        }

        public TileSpec GetTileSpec(int id)
        {
            TileSpec spec;
            if (!Tiles.TryGetValue(id, out spec))
                throw new Exception($"Tile spec {id} does not exists");
            return spec;
        }
    }
}
