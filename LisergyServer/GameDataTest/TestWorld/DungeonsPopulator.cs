using Game.Engine.DataTypes;
using Game.Systems.Dungeon;
using Game.World;
using GameDataTest;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, ServerChunkMap map, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null)
            {
                var dungeon = (DungeonEntity)w.Game.Entities.CreateEntity(GameId.ZERO, EntityType.Dungeon);
                var spec = w.Game.Specs.Dungeons[TestDungeons.EASY.SpecId];
                dungeon.BuildFromSpec(spec);
                w.Game.Systems.Map.GetLogic(dungeon).SetPosition(place);
            }
        }
    }
}
