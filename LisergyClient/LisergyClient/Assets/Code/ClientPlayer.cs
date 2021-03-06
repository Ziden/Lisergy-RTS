
using Game;
using System.Collections.Generic;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {
        public ClientPlayer() : base()
        {
            StackLog.Debug("Created new player");
        }

        public Dictionary<string, WorldEntity> KnownOwnedEntities { get {
                Dictionary<string, WorldEntity> entities = new Dictionary<string, WorldEntity>();
                foreach (var building in Buildings)
                    entities[building.Id] = building;
                foreach (var party in Parties)
                    if(party != null)
                        entities[party.Id] = party;
                return entities;
            }
        }

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev){}
    }
}
