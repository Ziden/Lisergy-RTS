using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using Game.Systems.Player;
using System;
using System.Xml.Linq;

namespace Game.Systems.Battler
{
    public class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<OffensiveActionEvent>(OnOffensiveAction);
            Game.Network.On<BattleResultPacket>(OnBattleResult);
        }

        private void OnOffensiveAction(IEntity attacker, ref BattleGroupComponent atkGroup, OffensiveActionEvent ev)
        {
            if(GetLogic(ev.Attacker).IsBattling || GetLogic(ev.Defender).IsBattling)
            {
                Log.Error($"Error battle {ev.Attacker} vs {ev.Defender} already battling");
                return;
            }
            var battleId = GetLogic(ev.Attacker).StartBattle(ev.Defender);
            Game.Network.SendToServer(new BattleTriggeredPacket(battleId, ev.Attacker, ev.Defender), ServerType.BATTLE);
        }

        private void OnBattleResult(BattleResultPacket packet)
        {
            var attackerEntity = packet.Header.Attacker.Entity;
            var defenderEntity = packet.Header.Defender.Entity;

            var finishEvent = new BattleFinishedEvent(packet.Header, packet.Turns);

            if (attackerEntity is IEntity e) e.Components.CallEvent(finishEvent);
            if (defenderEntity is IEntity e2) e2.Components.CallEvent(finishEvent);

            var atkPlayer = Game.Players.GetPlayer(attackerEntity.OwnerID);
            var defPlayer = Game.Players.GetPlayer(defenderEntity.OwnerID);

            if (atkPlayer != null)
            {
                Game.Network.SendToPlayer(attackerEntity.GetUpdatePacket(atkPlayer), atkPlayer);
                if (!Game.Logic.BattleGroup(defenderEntity).IsDestroyed)
                    Game.Network.SendToPlayer(defenderEntity.GetUpdatePacket(atkPlayer), atkPlayer);
            }

            if (defPlayer != null)
            {
                Game.Network.SendToPlayer(defenderEntity.GetUpdatePacket(defPlayer), defPlayer);
                if (!Game.Logic.BattleGroup(defenderEntity).IsDestroyed)
                    Game.Network.SendToPlayer(attackerEntity.GetUpdatePacket(defPlayer), defPlayer);

            }
        }

        private void OnBattleFinish(IEntity e, ref BattleGroupComponent component, BattleFinishedEvent ev)
        {
            var logic = GetLogic(e);
            logic.ClearBattleId();
            if (logic.IsDestroyed)
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
