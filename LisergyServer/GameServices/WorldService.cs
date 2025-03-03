using Game.Engine;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.Network.ServerPackets;
using Game.Systems.BattleGroup;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;
using System.Linq;

namespace Game.Services
{
    public class WorldService : IEventListener
    {
        private GameWorld _world;
        private IGameNetwork _network;

        public WorldService(LisergyGame game)
        {
            _world = (GameWorld)game.World;
            _network = game.Network;
            game.Events.On<EntityRemovedFromMapEvent>(this, OnEntityRemoved);
            game.Events.On<BattleTriggeredEvent>(this, OnBattleTrigger);

            // Coming back from battle server
            game.Network.OnInput<BattleResultPacket>(OnBattleResultPacket);
        }

        private void OnEntityRemoved(EntityRemovedFromMapEvent ev)
        {
            _network.SendToPlayer(new EntityDestroyPacket(ev.Entity), ev.Tile.Logic.Vision.GetPlayersViewing().ToArray());
        }


        /// <summary>
        /// Received from battle server
        /// </summary>
        private void OnBattleResultPacket(BattleResultPacket packet)
        {
            var atkPlayer = _world.Game.Players[packet.Header.Attacker.OwnerID];
            var defPlayer = _world.Game.Players[packet.Header.Defender.OwnerID];

            var attackerEntity = _world.Game.Entities[packet.Header.Attacker.EntityId];
            var defenderEntity = _world.Game.Entities[packet.Header.Defender.EntityId];

            // TODO: Move to system, this is logic
            if (atkPlayer != null) atkPlayer.EntityLogic.RecordBattleHeader(packet.Header);
            if (defPlayer != null) defPlayer.EntityLogic.RecordBattleHeader(packet.Header);

            var attackerGroup = attackerEntity.Components.Get<BattleGroupComponent>();
            var defenderGroup = defenderEntity.Components.Get<BattleGroupComponent>();

            attackerGroup.Units = packet.Header.Attacker.Units;
            defenderGroup.Units = packet.Header.Defender.Units;

            attackerEntity.Save(attackerGroup);
            defenderEntity.Save(defenderGroup);

            var finishEvent = EventPool<BattleFinishedEvent>.Get();
            finishEvent.Battle = packet.Header.BattleID;
            finishEvent.Header = packet.Header;
            finishEvent.Turns = packet.Turns;

            if (attackerEntity is IEntity e) e.Components.CallEvent(finishEvent);
            if (defenderEntity is IEntity e2) e2.Components.CallEvent(finishEvent);

            if (atkPlayer != null)
            {
                _network.DeltaCompression.SendEntityPacket(attackerEntity.EntityId, atkPlayer.EntityId);
                if (!defenderEntity.Logic.BattleGroup.IsDestroyed)
                    _network.DeltaCompression.SendEntityPacket(defenderEntity.EntityId, atkPlayer.EntityId);
            }

            if (defPlayer != null)
            {
                _network.DeltaCompression.SendEntityPacket(defenderEntity.EntityId, defPlayer.EntityId);

                if (!defenderEntity.Logic.BattleGroup.IsDestroyed)
                    _network.DeltaCompression.SendEntityPacket(attackerEntity.EntityId, defPlayer.EntityId);
            }
            EventPool<BattleFinishedEvent>.Return(finishEvent);
        }


        private void OnBattleTrigger(BattleTriggeredEvent ev)
        {
            var attackerEntity = _world.Game.Entities[ev.Attacker.EntityId];
            var defenderEntity = _world.Game.Entities[ev.Defender.EntityId];
            _world.Game.Network.SendToServer(new BattleQueuedPacket(ev.BattleID, attackerEntity, defenderEntity), ServerType.BATTLE);
        }
    }
}
