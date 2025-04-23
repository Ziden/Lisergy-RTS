using System;
using System.Collections.Generic;
using Game.Engine.ECLS;

namespace Game.Systems.Tile
{
	[Serializable]
	public class TileHabitantsComponent : IComponent
	{
		public IEntity Building;
		public List<IEntity> EntitiesIn = new List<IEntity>(); // TODO: Use Gameids !!!

		public override string ToString()
		{
			return $"<TileHabitantsComponent EntitiesIn={EntitiesIn.Count} Building={Building.EntityId}>";
		}
	}
}