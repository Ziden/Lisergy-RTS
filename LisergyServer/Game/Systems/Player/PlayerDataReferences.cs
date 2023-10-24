using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Player
{
    /// <summary>
    /// Keeps track of the player data
    /// </summary>
    [Serializable]
    public class PlayerData : IReferenceComponent
    {
        /// <summary>
        /// All entities owned by the player in the current world
        /// </summary>
        public Dictionary<EntityType, List<GameId>> OwnedEntities = new Dictionary<EntityType, List<GameId>>()
        {
            { EntityType.Party, new List<GameId>() },
            { EntityType.Building, new List<GameId>() }
        };

        /// <summary>
        /// Keeps track of all units that are not deployed in parties
        /// </summary>
        public Dictionary<GameId, Unit> StoredUnits = new Dictionary<GameId, Unit>();

        /// <summary>
        /// Battle headers of this player battles
        /// </summary>
        public List<BattleHeader> BattleHeaders = new List<BattleHeader>();
    }

    public class VisibilityReferences : IReferenceComponent
    {
        /// <summary>
        /// All tiles that the player is currently seeing
        /// </summary>
        public HashSet<Position> VisibleTiles = new HashSet<Position>();

        /// <summary>
        /// All tiles that the player has seen at least once
        /// </summary>
        public HashSet<Position> OnceExplored = new HashSet<Position>();
    }
}
