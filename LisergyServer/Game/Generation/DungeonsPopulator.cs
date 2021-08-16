using Game.Battles;
using Game.Entity;
using Game.Inventories;
using Game.World;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null) {
                var dungeon = new Dungeon();
                var enemy = new Unit(0);
                enemy.Name = "Bandit";
                enemy.SetSpecStats();
                dungeon.AddBattle(enemy);
                dungeon.Rewards = new Item[] { new Item(1, 10) }; // gold 
                dungeon.Tile = place;
            }
        }
    }
}
