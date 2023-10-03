using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using System;

namespace Game.Systems.Battler
{
    public class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(GameLogic game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<OffensiveActionEvent>(OnOffensiveAction);
        }

        private void OnOffensiveAction(IEntity attacker, BattleGroupComponent atkGroup, OffensiveActionEvent ev)
        {
            var battleID = Guid.NewGuid();
            var e = attacker as BaseEntity;
            var start = new BattleStartPacket(battleID, e.X, e.Y, new BattleTeam(ev.Attacker, ev.AttackerGroup.Units.ToArray()), new BattleTeam(ev.Defender, ev.DefenderGroup.Units.ToArray()));
            Game.NetworkPackets.Call(start);
            if (ev.Attacker.Owner != null && ev.Attacker.Owner.CanReceivePackets()) ev.Attacker.Owner.Send(start);
            if (ev.Defender.Owner != null && ev.Defender.Owner.CanReceivePackets()) ev.Defender.Owner.Send(start);  
        }

        private static void OnBattleFinish(IEntity e, BattleGroupComponent component, BattleFinishedEvent ev)
        {
            component.BattleID = GameId.ZERO;
            if (component.IsDestroyed)
            {
                if(e is DungeonEntity d) d.Tile = null;
                if (e is PartyEntity p)
                {
                    p.Tile = e.Owner.GetCenter().Tile;
                    foreach (var unit in component.Units)
                        unit.HealAll();
                }
            }

        }
    }
}
