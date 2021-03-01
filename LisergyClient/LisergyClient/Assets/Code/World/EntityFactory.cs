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
            if (entity is Party)
            {
                var party = (Party)entity;
                var clientParty = new ClientParty(owner, party);
                owner.Parties[party.PartyIndex] = clientParty;
                return clientParty;
            } else if (entity is Building)
            {
                var building = (Building)entity;
                var clientBuilding = new ClientBuilding(building.SpecID, owner, tile);
                tile.Building = clientBuilding;
                if (building.SpecID == StrategyGame.Specs.InitialBuilding && clientBuilding.IsMine())
                    CameraBehaviour.FocusOnTile(tile);
                return clientBuilding;
            }
            throw new Exception($"Entity Factor does not know how to instantiate {entity.GetType().Name}");
        }
    }
}
