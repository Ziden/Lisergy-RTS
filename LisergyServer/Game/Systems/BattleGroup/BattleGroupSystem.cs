using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using System;

namespace Game.Systems.Battler
{
    public class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<OffensiveActionEvent>(OnOffensiveAction);
        }

        private void OnOffensiveAction(IEntity attacker, BattleGroupComponent atkGroup, OffensiveActionEvent ev)
        {
            var battleID = Guid.NewGuid();
            var start = new BattleStartPacket(battleID, ev.Attacker, ev.Defender);
            Game.Network.IncomingPackets.Call(start);
            Players.GetPlayer(attacker.OwnerID)?.Send(start);
            Players.GetPlayer(attacker.OwnerID)?.Send(start);  
        }

        private void OnBattleFinish(IEntity e, BattleGroupComponent component, BattleFinishedEvent ev)
        {
         
            component.BattleID = GameId.ZERO;
            if (GameLogic.EntityLogic(e).BattleGroup.IsDestroyed)
            {
                e.Components.CallEvent(new GroupDeadEvent()
                {
                    GroupComponent = component,
                    Entity = e
                });
            }
        }
    }
}
