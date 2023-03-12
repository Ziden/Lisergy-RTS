using Game;
using Game.Entity;
using System;

namespace Assets.Code.World
{
    public class ClientEntityManager
    {

        public static event Action<ClientParty> OnPartyUpdated;
        public static event Action<ClientBuilding> OnBuildingUpdated;
        public static event Action<ClientDungeon> OnDungeonUpdated;

        public static WorldEntity InstantiateOrUpdateEntity(ClientStrategyGame game, WorldEntity serverEntity)
        {
            var owner = game.GetWorld().GetOrCreateClientPlayer(serverEntity.OwnerID);
            serverEntity.Owner = owner;

            var targetTile = (ClientTile)game.GetWorld().GetTile(serverEntity.X, serverEntity.Y);
            var knownEntity = owner.GetKnownEntity(serverEntity.Id);


            if (serverEntity is Party serverParty)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Party, ClientParty>(serverParty);
                OnPartyUpdated?.Invoke(clientEntity);
                return clientEntity;

            } else if (serverEntity is Building serverBuilding)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Building, ClientBuilding>(serverBuilding);
                OnBuildingUpdated?.Invoke(clientEntity);
                return clientEntity;
            }
            else if (serverEntity is Dungeon serverDungeon)
            {
                var clientEntity = owner.EnsureInstantiatedAndKnown<Dungeon, ClientDungeon>(serverDungeon);
                OnDungeonUpdated?.Invoke(clientEntity);
                return clientEntity;
            } else
                throw new Exception($"Entity Factory does not know how to instantiate {serverEntity.GetType().Name}");
        }
    }
}
