using Game;
using Game.Engine.ECLS;
using PlayFab;
using WebGameLogic;

namespace WebPlayerLogic.Playfab
{

    public class EntityPersistence
    {
        private LisergyGame _game;

        public EntityPersistence(LisergyGame game)
        {
            _game = game;
        }

        public async Task<T> LoadEntity<T>(string playerId)
        {
            var r = await PlayFabServerAPI.GetUserReadOnlyDataAsync(new PlayFab.ServerModels.GetUserDataRequest()
            {
                PlayFabId = playerId,
                Keys = new string[] { typeof(T).FullName }.ToList()
            });
            if (r.Error != null) throw new Exception(r.Error.GenerateErrorReport());
            var d = WebSerializer.Deserialize<SerializedEntity>(r.Result.Data[typeof(T).FullName].Value);
            var e = _game.Entities.CreateEntity(d.EntityType);
            foreach (var c in d.Components)
            {
                e.Components.Save(c);
            }
            e.Logic.DeltaCompression.Clear();
            return (T)e;
        }

        public async Task SaveEntity(string playerId, IEntity entity)
        {
            var val = new SerializedEntity(entity);
            var r = await PlayFabServerAPI.UpdateUserReadOnlyDataAsync(new PlayFab.ServerModels.UpdateUserDataRequest()
            {
                PlayFabId = playerId,
                Data = new Dictionary<string, string>()
                {
                    { entity.GetType().FullName, WebSerializer.Serialize(val) }
                },
            });
            if (r.Error != null) throw new Exception(r.Error.GenerateErrorReport());
        }
    }
}
