using System;
using System.Collections.Generic;

namespace GameData
{
    public class GameSpec
    {
        public BuildingSpec InitialBuilding;

        public Dictionary<byte, BuildingSpec> Buildings = new Dictionary<byte, BuildingSpec>();

        public BuildingSpec GetBuildingSpec(byte id)
        {
            BuildingSpec spec;
            if (!Buildings.TryGetValue(id, out spec))
                throw new Exception($"Building spec {id} does not exists");
            return spec;
        }
    }
}
