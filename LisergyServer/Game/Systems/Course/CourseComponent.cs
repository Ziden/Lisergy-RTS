using Game.DataTypes;
using Game.ECS;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Movement
{
    /// <summary>
    /// Defines that this entity is able to set courses to move
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    [SyncedComponent]
    public struct CourseComponent : IComponent
    {
        public GameId CourseId;
        public CourseIntent MovementIntent;

        public override string ToString()
        {
            return $"<EntityMovementComponent Course={CourseId}>";
        }
    }
}
