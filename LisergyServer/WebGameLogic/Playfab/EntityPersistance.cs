using Game;
using Game.Engine.ECLS;
using PlayFab;
using PlayFab.ServerModels;
using WebGameLogic;

namespace WebPlayerLogic.Playfab;

public class EntityPersistence
{
	private readonly LisergyGame _game;

	public EntityPersistence(LisergyGame game)
	{
		_game = game ?? throw new ArgumentNullException(nameof(game));
	}

	public async Task<T> LoadEntity<T>(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
			throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

		var r = await PlayFabServerAPI.GetUserReadOnlyDataAsync(new GetUserDataRequest
		{
			PlayFabId = playerId,
			Keys = new List<string> {typeof(T).FullName ?? throw new InvalidOperationException("Type name is null")}
		});

		if (r.Error != null) throw new InvalidOperationException(r.Error.GenerateErrorReport());

		if (!r.Result.Data.TryGetValue(typeof(T).FullName!, out var dataValue) || dataValue == null)
			throw new KeyNotFoundException($"Entity of type {typeof(T).FullName} not found for player {playerId}");

		var d = WebSerializer.Deserialize<SerializedEntity>(dataValue.Value)
		        ?? throw new InvalidOperationException("Failed to deserialize entity");

		var e = _game.Entities.CreateEntity(d.EntityType);
		foreach (var c in d.Components) e.Components.Save(c);
		e.Logic.DeltaCompression.Clear();
		return (T) e;
	}

	public async Task SaveEntity(string playerId, IEntity entity)
	{
		if (string.IsNullOrEmpty(playerId))
			throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

		if (entity == null) throw new ArgumentNullException(nameof(entity));

		var entityTypeName = entity.GetType().FullName
		                     ?? throw new InvalidOperationException("Entity type name is null");

		var val = new SerializedEntity(entity);
		var r = await PlayFabServerAPI.UpdateUserReadOnlyDataAsync(new UpdateUserDataRequest
		{
			PlayFabId = playerId,
			Data = new Dictionary<string, string>
			{
				{entityTypeName, WebSerializer.Serialize(val)}
			}
		});

		if (r.Error != null) throw new InvalidOperationException(r.Error.GenerateErrorReport());
	}
}