using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class GameSpec
    {
        public int Version = 1;

        public byte InitialBuilding;
        public ushort InitialUnit;

        public Dictionary<byte, BuildingSpec> Buildings = new Dictionary<byte, BuildingSpec>();
        public Dictionary<ushort, TileSpec> Tiles = new Dictionary<ushort, TileSpec>();
        public Dictionary<ushort, UnitSpec> Units = new Dictionary<ushort, UnitSpec>();

        public BuildingSpec GetBuildingSpec(byte id)
        {
            BuildingSpec spec;
            if (!Buildings.TryGetValue(id, out spec))
                throw new Exception($"Building spec {id} does not exists");
            return spec;
        }

        public TileSpec GetTileSpec(ushort id)
        {
            TileSpec spec;
            if (!Tiles.TryGetValue(id, out spec))
                throw new Exception($"Tile spec {id} does not exists");
            return spec;
        }
    }
}
