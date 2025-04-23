using System;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;

namespace Game.Systems.Player
{
    /// <summary>
    ///     Represents basic data of a player profile that will be shared with the client
    /// </summary>
    [Serializable]
	public class PlayerProfileComponent : IComponent
	{
		public string Name;
		public GameId PlayerId;

		public PlayerProfileComponent(in GameId id)
		{
			PlayerId = id;
		}

		public override string ToString()
		{
			return $"<Profile PlayerId={PlayerId} Name={Name}/>";
		}
	}
}