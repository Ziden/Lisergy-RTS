using Game;
using Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.World
{
    public class EntityFactory
    {

        public static WorldEntity InstantiateClientEntity(WorldEntity entity, ClientPlayer owner, ClientTile tile)
        {
            var knownEntity = owner.GetKnownEntity(entity.Id);
            if (entity is Party)
            {
                var party = (Party)entity;
                var clientParty = knownEntity==null ? new ClientParty(owner, party) : (ClientParty)knownEntity;
                owner.Parties[party.PartyIndex] = clientParty;
                clientParty.Id = party.Id;
                clientParty.GetGameObject().SetActive(true);
                owner.KnowAbout(clientParty);
                return clientParty;
            } else if (entity is Building)
            {
                var building = (Building)entity;
                var clientBuilding = knownEntity==null ?new ClientBuilding(building.SpecID, owner, tile) : (ClientBuilding)knownEntity;
                clientBuilding.Id = building.Id;
                tile.StaticEntity = clientBuilding;
                if (building.SpecID == StrategyGame.Specs.InitialBuilding && clientBuilding.IsMine())
                    CameraBehaviour.FocusOnTile(tile);
                clientBuilding.GetGameObject().SetActive(true);
                owner.KnowAbout(clientBuilding);
                return clientBuilding;
            }
            else if (entity is Dungeon)
            {
                var clientDungeon = knownEntity == null ? new ClientDungeon((Dungeon)entity, tile) : (ClientDungeon)knownEntity;
                tile.StaticEntity = clientDungeon;
                owner.KnowAbout(clientDungeon);
                clientDungeon.GetGameObject().SetActive(true);
                return clientDungeon;
            } else
                throw new Exception($"Entity Factory does not know how to instantiate {entity.GetType().Name}");
        }
    }
}
