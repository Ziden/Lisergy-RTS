using ClientSDK.SDKEvents;
using Game;
using Game.DataTypes;

using Game.Events.ServerEvents;

namespace ClientSDK.Services
{
    public interface IEntityModule : IClientModule
    {
        
    }

    public class EntityModule : IEntityModule
    {
        private IGameClient _client;

        public EntityModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Network.On<EntityUpdatePacket>(OnEntityUpdate);
        }

        private void OnEntityUpdate(EntityUpdatePacket packet) 
        {
            var existingEntity = _client.Game.Entities[packet.EntityId];
            if(existingEntity == null)
            {
                GameId.NextGeneration = packet.EntityId;
                existingEntity = _client.Game.Entities.CreateEntity(packet.OwnerId, packet.Type);
                _client.Modules.Views.GetOrCreateView(existingEntity);
                existingEntity.Components.CallEvent(new ClientAwareOfEntityEvent());
                Log.Debug($"Client now aware of entity entity {existingEntity}");
            }
            _client.Modules.Components.UpdateComponents(existingEntity, packet.SyncedComponents);
        }
    }
}
