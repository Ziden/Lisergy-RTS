using Game.ECS;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Tile
{
    public class TileHabitants : IComponent
    {
        public List<WorldEntity> EntitiesIn = new List<WorldEntity>();
        public IEntity Building;
    }
}
