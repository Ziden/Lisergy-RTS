using Game.ECS;
using System;

namespace Game.Systems.Movement
{
    /// <summary>
    /// Defines the entity movement speed
    /// </summary>
    [Serializable]
    [SyncedComponent]
    public struct MovespeedComponent : IComponent
    {
        public TimeSpan MoveDelay;

        public override string ToString()
        {
            return $"<EntityMovementComponent MoveDelay={MoveDelay.TotalSeconds}>";
        }
    }
}
