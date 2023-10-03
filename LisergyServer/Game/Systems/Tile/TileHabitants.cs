using Game.ECS;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Tile
{
    public class TileHabitants : IComponent
    {
        public List<BaseEntity> EntitiesIn = new List<BaseEntity>();
        public IEntity Building;
    }
}
