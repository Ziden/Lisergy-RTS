using System;
using Game.Engine.Network;
using Game.World;
using GameData;

namespace Game.Systems.Building
{
	[Serializable]
	public class BuildCommand : BasePacket, IGameCommand
	{
		public BuildingSpecId Building;
		public Location Location;

		public void Execute(IGame game)
		{
			var tile = game.World.GetTile(Location);
			if (tile.Logic.Building.IsTileFreeForBuilding(Building) != BuildResult.Ok) throw new Exception("Bad tile");
			var tech = Sender.EntityLogic.CheckTechTree(Building);
			if (tech.Status != BuildingTechStatus.Available) throw new Exception("Blocked by spec " + tech.BlockedBy);
			if (tile.Logic.Building.PlaceConstruction(Building, SenderPlayerId) == null)
				throw new Exception("Failed to place building");
		}
	}
}