﻿using Game.Battles;
using Game.Entity;
using Game.Inventories;
using Game.World;
using GameDataTest;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null) {
                var dungeon = Dungeon.BuildFromSpec(TestDungeons.EASY.DungeonSpecID);
                dungeon.Tile = place;
            }
        }
    }
}