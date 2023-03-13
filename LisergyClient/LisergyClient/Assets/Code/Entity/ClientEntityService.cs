using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;

namespace Assets.Code.World
{
    public class ClientEntityService : IEventListener
    {
        private ClientStrategyGame _game;

        public static event Action<ClientParty> OnPartyUpdated;
        public static event Action<ClientBuilding> OnBuildingUpdated;
        public static event Action<ClientDungeon> OnDungeonUpdated;

        public ClientEntityService(ClientStrategyGame game)
        {
            _game = game;
            _game.NetworkEvents.Register<EntityDestroyPacket>(this, EntityDestroy);
            _game.NetworkEvents.Register<EntityMovePacket>(this, EntityMove);
            _game.NetworkEvents.Register<EntityUpdatePacket>(this, EntityUpdate);
        }

        [EventMethod]
        public void EntityDestroy(EntityDestroyPacket ev)
        {
            Log.Debug("Received entity destroy");
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.ID);
            if (knownEntity == null)
                throw new System.Exception($"Server sent destroy event for entity {ev.ID} from {ev.OwnerID} at however its not visible to client");

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
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.ID);
            if (knownEntity == null)
                throw new Exception($"Server sent move event for entit3y {ev.ID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");

            var newTile = _game.GetWorld().GetClientTile(ev.X, ev.Y);
            knownEntity.Tile = newTile;
        }

        [EventMethod]
        public void EntityUpdate(EntityUpdatePacket ev)
        {
            Log.Debug("Received entity update");
            var serverEntity = ev.Entity;
            var owner = _game.GetWorld().GetOrCreateClientPlayer(serverEntity.OwnerID);
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
