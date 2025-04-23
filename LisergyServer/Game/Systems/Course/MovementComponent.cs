using System;
using System.Runtime.InteropServices;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;

namespace Game.Systems.Movement
{
    /// <summary>
    ///     Defines that this entity is able to set courses to move
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Serializable]
	[SyncedComponent]
	public struct MovementComponent : IComponent
	{
		public GameId CourseId;
		public CourseIntent MovementIntent;

		public override string ToString()
		{
			return $"<EntityMovementComponent Course={CourseId}>";
		}
	}
}