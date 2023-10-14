using Game;
using Game.Events.ServerEvents;

namespace ClientSDK.Services
{
    public interface IEntityModule : IClientModule
    {

    }

    public class EntityModule : IEntityModule
    {
        private IGameClient _client;
        private IGameEntities _entities;

        public EntityModule(IGameClient client)
        {
            _client = client;
            _entities = client.Game.Entities;
        }

        public void Register()
        {
            _client.Network.On<EntityUpdatePacket>(OnEntityUpdate);
        }

        public void RegisterEntity<EntityType>()
        {
            throw new System.NotImplementedException();
        }

        private void OnEntityUpdate(EntityUpdatePacket packet) 
        {
            var existingEntity = _entities[packet.EntityId];
            if(existingEntity == null)
            {
                //_entities.CreateEntity()
            }
            
        }
    }
}
