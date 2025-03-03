using Game.Engine.ECLS;
using System;
using System.Collections.Generic;

namespace Game.Systems.Tile
{
    [Serializable]
    public class TileHabitantsComponent : IComponent
    {
        public List<IEntity> EntitiesIn = new List<IEntity>(); // TODO: Use Gameids !!!
        public IEntity Building;

        public override string ToString()
        {
            return $"<TileHabitantsComponent EntitiesIn={EntitiesIn.Count} Building={Building.EntityId}>";
        }
    }
}
