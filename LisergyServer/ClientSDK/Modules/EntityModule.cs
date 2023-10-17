using ClientSDK.Data;
using ClientSDK.Modules;
using ClientSDK.SDKEvents;
using Game;
using Game.DataTypes;

using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Systems.Map;
using Game.Systems.Movement;
using System.Linq;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service that controls entities that the game client is aware of.
    /// Expose basic entity control functionality like moving or taking entity actions.
    /// </summary>
    public interface IEntityModule : IClientModule
    {

    }

    public class EntityModule : IEntityModule
    {
        private IGameClient _client;
        private ComponentsModule _entityComponents;

        public EntityModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _entityComponents = (ComponentsModule)_client.Modules.Components;
            _client.Network.On<EntityUpdatePacket>(OnEntityUpdate);
        }

        private void OnEntityUpdate(EntityUpdatePacket packet) 
        {
            var existingEntity = _client.Game.Entities[packet.EntityId];
            bool entityCreated = false;
            if (existingEntity == null)
            {
                GameId.NextGeneration = packet.EntityId;
                existingEntity = _client.Game.Entities.CreateEntity(packet.OwnerId, packet.Type);
                entityCreated = true;
                Log.Debug($"Client now aware of entity entity {existingEntity}");
            }
            Log.Debug($"Received entity update for {existingEntity}");
         
            _entityComponents.UpdateComponents(existingEntity, packet.SyncedComponents);
            var view = _client.Modules.Views.GetOrCreateView(existingEntity);
            if (view.State == EntityViewState.NOT_RENDERED) view.RenderView();
            if(entityCreated)
            {
                _client.ClientEvents.Call(new ClientAwareOfEntityEvent()
                {
                    Entity = existingEntity,
                    View = _client.Modules.Views.GetOrCreateView(existingEntity)
                });
            }
        }
    }
}
