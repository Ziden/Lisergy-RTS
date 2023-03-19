
using Assets.Code.Entity;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using Game.Entity.Entities;
using Game.Events.GameEvents;
using System.Collections.Generic;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {
        public Dictionary<GameId, WorldEntity> KnowsAbout = new Dictionary<GameId, WorldEntity>();

        public ClientPlayer() : base()
        {
            StackLog.Debug("Created new player");
        }

        public ViewType UpdateClientState<EntityType, ViewType>(EntityType serverEntity, List<IComponent> components) where EntityType : WorldEntity where ViewType : EntityView<EntityType>
        {
            var clientEntity = (EntityType)GetKnownEntity(serverEntity.Id);
            if (clientEntity == null)
            {
                clientEntity = InstanceFactory.CreateInstance<EntityType, PlayerEntity>(serverEntity.Owner);
                clientEntity.Id = serverEntity.Id;
                EntitySynchronizer.SyncComponents(clientEntity, components);
                var view = InstanceFactory.CreateInstance<ViewType, EntityType>(clientEntity);   
                GameView.Controller.AddView(clientEntity, view);
                clientEntity.Components.Add(view);
                EntitySynchronizer.HookBaseEvents(clientEntity);
                view.OnUpdate(serverEntity, components);
                KnowsAbout[serverEntity.Id] = clientEntity;
                return view;
            }
            else
            {
                EntitySynchronizer.SyncComponents(clientEntity, components);
                var view = GameView.GetView<ViewType>(clientEntity);
                view.OnUpdate(serverEntity, components);
                return view;
            }
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
