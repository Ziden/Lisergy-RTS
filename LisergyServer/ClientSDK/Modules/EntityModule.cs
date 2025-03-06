using ClientSDK.Data;
using ClientSDK.Sync;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Tile;
using Game.World;
using System;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service that controls entities that the game client is aware of.
    /// Expose basic entity control functionality like moving or taking entity actions.
    /// </summary>
    public interface IEntityModule : IClientModule
    {
        /// <summary>
        /// Will be called when any update of the given component (add, remove, change) is done
        /// </summary>
        void OnUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent;

        /// <summary>
        /// Registers a component sync. 
        /// Whenever the given entity type has the given component type updated, instead of the values simply being copied
        /// the sync code will be called.
        /// The callback has the Entity, OLD VALUE and NEW VALUE parameters.
        /// </summary>
        void OnComponentModified<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent;

        /// <summary>
        /// Registers a component removed callback. 
        /// Whenever the given entity type has the given component type removed, the callback will be called after the operation has been done..
        /// The callback has the Entity, OLD VALUE and NEW VALUE parameters.
        /// </summary>
        void OnComponentRemoved<ComponentType>(Action<IEntity, ComponentType> OnRemoved) where ComponentType : IComponent;


        /// <summary>
        /// Registers a component removed callback. 
        /// Whenever the given entity type has the given component type removed, the callback will be called after the operation has been done..
        /// The callback has the Entity, OLD VALUE and NEW VALUE parameters.
        /// </summary>
        void OnComponentAdded<ComponentType>(Action<IEntity, ComponentType> OnAdded) where ComponentType : IComponent;

        /// <summary>
        /// Removes all event callbacks from the given object
        /// </summary>
        void RemoveListener(object listener);
    }

    public class EntityModule : IEntityModule
    {
        private GameClient _client;
        public ComponentSynchronizer ComponentSync { get; private set; }
        public SystemSynchronizer SystemSync { get; private set; }

        public EntityModule(GameClient client)
        {
            _client = client;
            ComponentSync = new ComponentSynchronizer(_client);
            SystemSync = new SystemSynchronizer(_client);
        }

        public void Register()
        {
            _client.Network.OnInput<EntityUpdatePacket>(OnEntityUpdate);
            SystemSync.ListenForRequiredSyncs();
        }

        private void OnEntityUpdate(EntityUpdatePacket packet)
        {
            var existingEntity = _client.Game.Entities[packet.EntityId];
            if (existingEntity == null)
            {
                if (packet.Type == EntityType.Tile)
                {
                    var tileData = packet.GetComponent<TileDataComponent>();
                    var chunk = _client.Game.World.GetTileChunk(tileData.Position);
                    var internalTileX = tileData.Position.X % GameWorld.CHUNK_SIZE;
                    var internalTileY = tileData.Position.Y % GameWorld.CHUNK_SIZE;
                    var existing = chunk.Tiles[internalTileX, internalTileY];
                    if (existing == null)
                    {
                        existing = chunk.CreateTile(internalTileX, internalTileY, packet.EntityId);
                    }
                    existingEntity = existing.Entity;
                }
                else
                {
                    existingEntity = _client.Game.Entities.CreateEntity(packet.Type, packet.OwnerId, packet.EntityId);
                }
            }
            _client.SDKLog.Debug($"Received entity update for {existingEntity}");
            var view = _client.Modules.Views.GetOrCreateView(existingEntity);
            ComponentSync.ProccessUpdate(existingEntity, packet.SyncedComponents, packet.RemovedComponentIds);
            if (view.State == EntityViewState.NOT_RENDERED)
            {
                view.RenderView();
            }
        }

        public void OnComponentModified<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent
        {
            ComponentSync.OnComponentModified(OnSync);
        }

        public void OnComponentRemoved<ComponentType>(Action<IEntity, ComponentType> OnRemoved) where ComponentType : IComponent
        {
            ComponentSync.OnComponentRemoved(OnRemoved);
        }

        public void OnUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnUpdated) where ComponentType : IComponent
        {
            ComponentSync.OnUpdate(OnUpdated);
        }

        public void OnComponentAdded<ComponentType>(Action<IEntity, ComponentType> OnAdded) where ComponentType : IComponent
        {
            ComponentSync.OnComponentAdded(OnAdded);
        }

        public void RemoveListener(object listener)
        {
            ComponentSync.RemoveListener(listener);
        }
    }
}
