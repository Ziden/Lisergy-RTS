using System;
using System.Collections.Generic;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using Game.World;

namespace Game.Systems.Player
{
    /// <summary>
    ///     Keeps track of the player data
    /// </summary>
    [Serializable]
	public class PlayerDataComponent : IComponent
	{
        /// <summary>
        ///     Battle headers of this player battles
        /// </summary>
        public List<BattleHeader> BattleHeaders = new List<BattleHeader>();

        /// <summary>
        ///     Keeps track of all units that are not deployed in parties
        /// </summary>
        public Dictionary<GameId, Unit> StoredUnits = new Dictionary<GameId, Unit>();
	}

	[Serializable]
	public class PlayerVisibilityComponent : IComponent
	{
        /// <summary>
        ///     All tiles that the player has seen at least once
        /// </summary>
        public List<Location> OnceExplored = new List<Location>();

        /// <summary>
        ///     All tiles that the player is currently seeing
        /// </summary>
        public List<Location> VisibleTiles = new List<Location>();
	}
}