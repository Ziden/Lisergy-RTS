using Game.Battles;
using Game.Entity;
using Game.World;
using System;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null) {
                var dungeon = new Dungeon();
                dungeon.SetBattles(new BattleTeam(new Unit(1))); // orc
                place.StaticEntity = dungeon;
            }
        }
    }
}
