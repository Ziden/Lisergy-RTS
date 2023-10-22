using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Player
{
    public class PlayerDataComponent : IReferenceComponent
    {
        public PartyEntity[] Parties;
        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<PlayerBuildingEntity> Buildings = new HashSet<PlayerBuildingEntity>();

        public HashSet<TileEntity> VisibleTiles = new HashSet<TileEntity>();
        public HashSet<TileEntity> OnceExplored = new HashSet<TileEntity>();
        public Dictionary<GameId, BattleState> BattleHeaders = new Dictionary<GameId, BattleState>();
    }
}
