using Game.ECS;
using System.Collections.Generic;

namespace Game.Tile
{
    public class TileHabitants : IComponent
    {
        public List<WorldEntity> EntitiesIn = new List<WorldEntity>();
        public WorldEntity Building;

        static TileHabitants()
        {
            SystemRegistry<TileHabitants, TileEntity>.AddSystem(new TileHabitantsSystem());
        }
    }
}
