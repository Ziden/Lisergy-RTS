using Game.DataTypes;
using System;

namespace Game.ECS
{

    public interface IEntity
    {
       public IComponentSet Components { get; }

        public GameId EntityId { get; }
    }
}
