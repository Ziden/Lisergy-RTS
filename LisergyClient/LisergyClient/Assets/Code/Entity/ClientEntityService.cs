using Game;
using Game.Building;
using Game.Dungeon;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Party;
using System;

namespace Assets.Code.World
{
    public class EntityListener : IEventListener
    {

        public static event Action<PartyView> OnPartyViewUpdated;
        public static event Action<PlayerBuildingView> OnBuildingViewUpdated;
        public static event Action<DungeonView> OnDungeonViewUpdated;

        public EntityListener(EventBus<ServerPacket> networkEvents)
        {
            networkEvents.Register<EntityDestroyPacket>(this, EntityDestroy);
            networkEvents.Register<EntityMovePacket>(this, EntityMove);
            networkEvents.Register<EntityUpdatePacket>(this, EntityUpdate);
        }

        [EventMethod]
        public void EntityDestroy(EntityDestroyPacket ev)
        {
            Log.Debug("Received entity destroy");
            var owner = GameView.World.GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.EntityID);
            if (knownEntity == null)
                throw new Exception($"Server sent destroy event for entity {ev.EntityID} from {ev.OwnerID} at however its not visible to client");

            knownEntity.Tile = null;
            var view = GameView.GetView(knownEntity);
            GameView.Destroy(view);
        }

        [EventMethod]
        public void EntityMove(EntityMovePacket ev)
        {
            Log.Debug("Received entity move");
            var owner = GameView.World.GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.EntityID);
            if (knownEntity == null)
                throw new Exception($"Server sent move event for entit3y {ev.EntityID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");

            var newTile = GameView.World.GetTile(ev.X, ev.Y);
            knownEntity.Tile = newTile;
        }

        [EventMethod]
        public void EntityUpdate(EntityUpdatePacket ev)
        {
            Log.Debug($"Received entity update {ev.Entity.GetType().Name} ({ev.SyncedComponents.Count} components)");
            var serverEntity = ev.Entity;
            var serverOwner = serverEntity.OwnerID;
            var owner = GameView.World.GetOrCreateClientPlayer(serverEntity.OwnerID);
            serverEntity.Owner = owner;

            // Move this to a client system
            if (serverEntity is PartyEntity serverParty)
            {
                var view = owner.UpdateClientState<PartyEntity, PartyView>(serverParty, ev.SyncedComponents);
                OnPartyViewUpdated?.Invoke(view);
            }

            else if (serverEntity is PlayerBuildingEntity serverBuilding)
            {
                var view = owner.UpdateClientState<PlayerBuildingEntity, PlayerBuildingView>(serverBuilding, ev.SyncedComponents);
                OnBuildingViewUpdated?.Invoke(view);
            }
            else if (serverEntity is DungeonEntity serverDungeon)
            {
                var view = owner.UpdateClientState<DungeonEntity, DungeonView>(serverDungeon, ev.SyncedComponents);
                OnDungeonViewUpdated?.Invoke(view);
            }

            else
                throw new Exception($"Entity Factory does not know how to instantiate {serverEntity.GetType().Name}");
        }
    }
}
