
using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {
        public Dictionary<GameId, WorldEntity> KnowsAbout = new Dictionary<GameId, WorldEntity>();

        public ClientPlayer() : base()
        {
            StackLog.Debug("Created new player");
        }

        public ClientType EnsureInstantiatedAndKnown<ServerType, ClientType>(ServerType serverEntity) where ServerType : WorldEntity where ClientType : WorldEntity, IGameObject, IClientEntity<ServerType, ClientType>
        {
            var known = GetKnownEntity(serverEntity.Id) as ClientType;
            if (known == null)
            {
                known = InstanceFactory.CreateInstance<ClientType, PlayerEntity>(this); // TODO if slow use a manual factory (e.g new ClientParty)
                known.InstantiateInScene(serverEntity);
                KnowsAbout[serverEntity.Id] = known;
            }
            else
            {
                known.UpdateData(serverEntity);
            }
            return known;
        }

        public WorldEntity GetKnownEntity(GameId id)
        {
            WorldEntity e;
            KnowsAbout.TryGetValue(id, out e);
            return e;
        }

        public void KnowAbout(WorldEntity e)
        {
            KnowsAbout[e.Id] = e;
        }

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev) { }
    }
}
