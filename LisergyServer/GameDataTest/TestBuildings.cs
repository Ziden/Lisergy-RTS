﻿using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestBuildings
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Buildings/{name}.prefab";

        public static void Generate(ref GameSpec spec)
        {

            // BUILDINGS
            // CASTLE
            spec.Buildings[1] = new BuildingSpec(
               1,
               new ArtSpec() { Address = Addr("SimpleHouse") },
               4
            );
            // FARM
            spec.Buildings[2] = new BuildingSpec(
                2,
                new ArtSpec() { Address = Addr("Farm") },
                4
            );
        }
    }
}