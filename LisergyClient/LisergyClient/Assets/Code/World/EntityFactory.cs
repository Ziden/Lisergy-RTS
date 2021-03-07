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
                clientParty.GameObject.SetActive(true);
                return clientParty;
            } else if (entity is Building)
            {
                var building = (Building)entity;
                var clientBuilding = knownEntity==null ?new ClientBuilding(building.SpecID, owner, tile) : (ClientBuilding)knownEntity;
                tile.Building = clientBuilding;
                if (building.SpecID == StrategyGame.Specs.InitialBuilding && clientBuilding.IsMine())
                    CameraBehaviour.FocusOnTile(tile);
                clientBuilding.GameObject.SetActive(true);
                return clientBuilding;
            }
            throw new Exception($"Entity Factor does not know how to instantiate {entity.GetType().Name}");
        }
    }
}
