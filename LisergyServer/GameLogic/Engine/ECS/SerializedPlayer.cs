using System;
using System.Collections.Generic;
using System.Linq;
using Game.Engine.DataTypes;
using Game.Systems.Player;
using Game.World;

namespace Game.Engine.ECLS
{
	[Serializable]
	public class SerializedPlayer
	{
		public PlayerDataComponent Data;
		public GameId PlayerId;
		public List<Location> SeenTiles;

		public SerializedPlayer()
		{
		}

		public SerializedPlayer(PlayerModel player)
		{
			PlayerId = player.EntityId;
			Data = player.Components.Get<PlayerDataComponent>();
			SeenTiles = player.Components.Get<PlayerVisibilityComponent>().OnceExplored.ToList();
		}
	}
}