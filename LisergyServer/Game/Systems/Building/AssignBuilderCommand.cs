using System;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.World;

namespace Game.Systems.Building
{
	[Serializable]
	public class AssignBuilderCommand : BasePacket, IGameCommand
	{
		public GameId EntityId;
		public Location Location;

		public void Execute(IGame game)
		{
			var entity = game.Entities[EntityId];
			var tile = game.World.GetTile(Location);
			var building = tile.Logic.Tile.GetBuildingOnTile();
			if (building == null || building.OwnerID != entity.OwnerID)
			{
				game.Log.Error("Assign building command failed - cannot build");
				return;
			}
			building.Logic.Building.AddBuilder(entity);
		}
	}
}