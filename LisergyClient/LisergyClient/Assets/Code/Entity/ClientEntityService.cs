using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;

namespace Assets.Code.World
{
    public class EntityListener : IEventListener
    {

        public static event Action<ClientParty> OnPartyUpdated;
        public static event Action<ClientBuilding> OnBuildingUpdated;
        public static event Action<ClientDungeon> OnDungeonUpdated;

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
            var owner = ClientStrategyGame.ClientWorld.GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.EntityID);
            if (knownEntity == null)
                throw new System.Exception($"Server sent destroy event for entity {ev.EntityID} from {ev.OwnerID} at however its not visible to client");

            var obj = knownEntity as IGameObject;
            MainBehaviour.Destroy(obj.GetGameObject());
            if (knownEntity.Tile.StaticEntity == knownEntity)
                knownEntity.Tile.StaticEntity = null;

            if (knownEntity is MovableWorldEntity)
                knownEntity.Tile.MovingEntities.Remove(knownEntity as MovableWorldEntity);
            knownEntity.Tile = null;
        }

        [EventMethod]
        public void EntityMove(EntityMovePacket ev)
        {
            Log.Debug("Received entity move");
            var owner = ClientStrategyGame.ClientWorld.GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.EntityID);
            if (knownEntity == null)
                throw new Exception($"Server sent move event for entit3y {ev.EntityID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");

            var newTile = ClientStrategyGame.ClientWorld.GetClientTile(ev.X, ev.Y);
            knownEntity.Tile = newTile;
        }

        [EventMethod]
        public void EntityUpdate(EntityUpdatePacket ev)
        {
            Log.Debug("Received entity update");
            var serverEntity = ev.Entity;
            var serverOwner = serverEntity.OwnerID;
            var owner = ClientStrategyGame.ClientWorld.GetOrCreateClientPlayer(serverEntity.OwnerID);
            serverEntity.Owner = owner;
            if (serverEntity is Party serverParty)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Party, ClientParty>(serverParty);
                OnPartyUpdated?.Invoke(clientEntity);
            }
            else if (serverEntity is Building serverBuilding)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Building, ClientBuilding>(serverBuilding);
                OnBuildingUpdated?.Invoke(clientEntity);
            }
            else if (serverEntity is Dungeon serverDungeon)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Dungeon, ClientDungeon>(serverDungeon);
                OnDungeonUpdated?.Invoke(clientEntity);
            }
            else
                throw new Exception($"Entity Factory does not know how to instantiate {serverEntity.GetType().Name}");
        }
    }
}
