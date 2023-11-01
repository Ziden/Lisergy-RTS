using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;

namespace Game.Systems.BattleGroup
{
    /// <summary>
    /// Called when a battle is finished processing
    /// </summary>
    public class BattleFinishedEvent : IGameEvent
    {
        public GameId Battle;
        public BattleHeader Header;
        public BattleTurnLog[] Turns;
    }

    /// <summary>
    /// Triggered when a battle is triggered in game logic
    /// </summary>
    public class BattleTriggeredEvent : IGameEvent
    {
        public TileVector Position;
        public GameId BattleID;
        public BattleTeamData Attacker;
        public BattleTeamData Defender;

        public BattleTriggeredEvent(GameId battleId, IEntity attacker, IEntity defender)
        {
            var pos = attacker.Get<MapPlacementComponent>();
            BattleID = battleId;
            Attacker = new BattleTeamData(attacker);
            Defender = new BattleTeamData(defender);
            Position = pos.Position;
        }
    }

    /// <summary>
    /// Fired when a entity with BattleGroupComponent finishes a battle and dies
    /// </summary>
    public class GroupDeadEvent : IGameEvent
    {
        public IEntity Entity;
    }

    /// <summary>
    /// When a unit is removed from an entity. 
    /// </summary>
    public class UnitRemovedEvent : IGameEvent
    {
        public Unit[] UnitsRemoved;
        public IEntity Entity;

        public UnitRemovedEvent(IEntity entity, params Unit[] units)
        {
            UnitsRemoved = units;
            Entity = entity;
        }
    }

    /// <summary>
    /// Triggered whenever a unit is added to agroup
    /// </summary>
    public class UnitAddToGroupEvent : IGameEvent
    {
        public IEntity Entity;
        public Unit Unit;
    }
}
