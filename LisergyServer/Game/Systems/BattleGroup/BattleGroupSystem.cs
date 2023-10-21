using Game.ECS;
using Game.Events;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using System.Linq;

namespace Game.Systems.Battler
{
    public unsafe class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<OffensiveActionEvent>(OnOffensiveAction);
            Game.Network.On<BattleResultPacket>(OnBattleResult);
        }

        /// <summary>
        /// When a entity moved offensively towards another entity
        /// </summary>
        private void OnOffensiveAction(IEntity attacker, OffensiveActionEvent ev)
        {
            if(GetLogic(ev.Attacker).IsBattling || GetLogic(ev.Defender).IsBattling)
            {
                Game.Log.Error($"Error battle {ev.Attacker} vs {ev.Defender} already battling");
                return;
            }
            var battleId = GetLogic(ev.Attacker).StartBattle(ev.Defender);
            Game.Network.SendToServer(new BattleQueuedPacket(battleId, ev.Attacker, ev.Defender), ServerType.BATTLE);
        }

        /// <summary>
        /// When received a battle finished processing from battle service
        /// </summary>
        private void OnBattleResult(BattleResultPacket packet)
        {
            var attackerEntity = Game.Entities[packet.Header.Attacker.EntityId];
            var defenderEntity = Game.Entities[packet.Header.Defender.EntityId];

            var attackerGroup = attackerEntity.Components.GetPointer<BattleGroupComponent>();
            var defenderGroup = defenderEntity.Components.GetPointer<BattleGroupComponent>();

            attackerGroup->Units = packet.Header.Attacker.Units;
            defenderGroup->Units = packet.Header.Defender.Units;

            var finishEvent = EventPool<BattleFinishedEvent>.Get();
            finishEvent.Battle = packet.Header.BattleID;
            finishEvent.Header = packet.Header;
            finishEvent.Turns = packet.Turns;

            if (attackerEntity is IEntity e) e.Components.CallEvent(finishEvent);
            if (defenderEntity is IEntity e2) e2.Components.CallEvent(finishEvent);

            var atkPlayer = Game.Players.GetPlayer(attackerEntity.OwnerID);
            var defPlayer = Game.Players.GetPlayer(defenderEntity.OwnerID);

            if (atkPlayer != null)
            {
                Game.Network.SendToPlayer(attackerEntity.GetUpdatePacket(atkPlayer), atkPlayer);
                if (!defenderEntity.EntityLogic.BattleGroup.IsDestroyed)
                    Game.Network.SendToPlayer(defenderEntity.GetUpdatePacket(atkPlayer), atkPlayer);
            }

            if (defPlayer != null)
            {
                Game.Network.SendToPlayer(defenderEntity.GetUpdatePacket(defPlayer), defPlayer);
                if (!defenderEntity.EntityLogic.BattleGroup.IsDestroyed)
                    Game.Network.SendToPlayer(attackerEntity.GetUpdatePacket(defPlayer), defPlayer);
            }

            EventPool<BattleFinishedEvent>.Return(finishEvent);
        }

        /// <summary>
        /// When a battle finished processing in game logic
        /// </summary>
        private void OnBattleFinish(IEntity e, BattleFinishedEvent ev)
        {
            var logic = GetLogic(e);
            logic.ClearBattleId();
            if (logic.IsDestroyed)
            {
                var groupDead = EventPool<GroupDeadEvent>.Get();
                groupDead.Entity = e;
                e.Components.CallEvent(groupDead);
                EventPool<GroupDeadEvent>.Return(groupDead);
            }
        }
    }
}
