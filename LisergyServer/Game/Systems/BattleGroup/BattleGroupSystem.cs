using Game.ECS;
using Game.Events;
using Game.Network.ServerPackets;
using Game.Systems.BattleGroup;
using Game.Systems.Course;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Tile;

namespace Game.Systems.Battler
{
    public unsafe class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<CourseFinishEvent>(OnCourseFinish);
            Game.Network.On<BattleResultPacket>(OnBattleResult);
        }


        private void OnCourseFinish(IEntity entity, CourseFinishEvent ev)
        {
            var tileHabitants = ev.ToTile.Components.GetReference<TileHabitantsReferenceComponent>();

            if (ev.Intent != CourseIntent.OffensiveTarget) return;
            if (tileHabitants.Building == null) return;

            if (!ev.Entity.Components.Has<BattleGroupComponent>() || !tileHabitants.Building.Components.Has<BattleGroupComponent>()) return;
            var atkGroup = ev.Entity.Components.Get<BattleGroupComponent>();
            var defGroup = tileHabitants.Building.Components.Get<BattleGroupComponent>();
            if (GetLogic(ev.Entity).IsBattling || GetLogic(tileHabitants.Building).IsBattling) return;
            var battleId = GetLogic(ev.Entity).StartBattle(tileHabitants.Building);
            Game.Network.SendToServer(new BattleQueuedPacket(battleId, ev.Entity, tileHabitants.Building), ServerType.BATTLE);
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
