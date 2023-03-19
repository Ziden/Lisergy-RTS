using Game.ECS;
using Game.World.Systems;
using System.Collections.Generic;

namespace Game.World.Components
{
    public class TileHabitants : IComponent
    {
        public List<WorldEntity> EntitiesIn = new List<WorldEntity>();
        public StaticEntity StaticEntity;

        static TileHabitants()
        {
            SystemRegistry<TileHabitants, Tile>.AddSystem(new EntityPlacementSystem());
        }
    }
}
