using Game.Engine.ECLS;
using System;

namespace Game.Systems.Movement
{
    /// <summary>
    /// Defines the entity movement speed
    /// </summary>
    [Serializable]
    [SyncedComponent]
    public class MovespeedComponent : IComponent
    {
        public TimeSpan MoveDelay;

        public override string ToString()
        {
            return $"<EntityMovementComponent MoveDelay={MoveDelay.TotalSeconds}>";
        }
    }
}
