using Assets.Code.Entity;
using Assets.Code.Views;
using Game;
using Game.Building;
using Game.Dungeon;
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Network.ServerPackets;
using Game.Party;
using Game.Player;
using System;
using System.Collections.Generic;

namespace Assets.Code.World
{
    public class EntityListener : IEventListener
    {

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
            var knownEntity = MainBehaviour.LocalPlayer.GetKnownEntity(ev.EntityID);
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
            var knownEntity = MainBehaviour.LocalPlayer.GetKnownEntity(ev.EntityID);
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

            // Make this generic ?
            if (serverEntity is PartyEntity serverParty)
            {
                var result = UpdateClientState<PartyEntity, PartyView>(serverParty, ev.SyncedComponents);
                if(result.Created)
                {
                    //OnPartyViewCreated?.Invoke(result.View);
                }
                ClientEvents.PartyViewUpdated(result.View);
            }

            else if (serverEntity is PlayerBuildingEntity serverBuilding)
            {
                var result = UpdateClientState<PlayerBuildingEntity, PlayerBuildingView>(serverBuilding, ev.SyncedComponents);
                if (result.Created)
                {
                    //OnBuildingViewCreated?.Invoke(result.View);
                }
                ClientEvents.BuildingViewUpdated(result.View);
            }
            else if (serverEntity is DungeonEntity serverDungeon)
            {
                var result = UpdateClientState<DungeonEntity, DungeonView>(serverDungeon, ev.SyncedComponents);
                if (result.Created)
                {
                   // OnDungeonViewCreated?.Invoke(result.View);
                }
                ClientEvents.DungeonViewUpdated(result.View);
            }

            else
                throw new Exception($"Entity Factory does not know how to instantiate {serverEntity.GetType().Name}");
        }

        public class EntityUpdateResult<ViewType, EntityType> where EntityType : WorldEntity where ViewType : EntityView<EntityType>
        {
            public ViewType View;
            public bool Created;
        }

        public EntityUpdateResult<ViewType, EntityType> UpdateClientState<EntityType, ViewType>(EntityType serverEntity, List<IComponent> components) where EntityType : WorldEntity where ViewType : EntityView<EntityType>
        {
            var localPlayer = MainBehaviour.LocalPlayer;
            var clientEntity = (EntityType)localPlayer.GetKnownEntity(serverEntity.Id);
            if (clientEntity == null)
            {
                clientEntity = InstanceFactory.CreateInstance<EntityType, PlayerEntity>(serverEntity.Owner);
                clientEntity.Id = serverEntity.Id;
                MovementEvents.HookMovementEvents(clientEntity);
                ComponentSynchronizer.SyncComponents(clientEntity, components);
                var view = InstanceFactory.CreateInstance<ViewType, EntityType>(clientEntity);
                GameView.Controller.AddView(clientEntity, view);
                clientEntity.Components.Add(view);
                view.OnUpdate(serverEntity, components);
                localPlayer.KnownEntities[serverEntity.Id] = clientEntity;
                return new EntityUpdateResult<ViewType, EntityType>()
                {
                    Created = true, View = view
                };
            }
            else
            {
                ComponentSynchronizer.SyncComponents(clientEntity, components);
                var view = GameView.GetView<ViewType>(clientEntity);
                view.OnUpdate(serverEntity, components);
                return new EntityUpdateResult<ViewType, EntityType>()
                {
                    Created = false,
                    View = view
                };
            }
        }
    }
}
