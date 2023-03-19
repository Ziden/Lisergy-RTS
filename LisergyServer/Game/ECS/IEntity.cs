using System;

namespace Game.ECS
{

    public interface IEntity
    {
       public IComponentSet Components { get; }
    }
}
