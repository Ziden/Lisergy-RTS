using System;
using System.Collections.Generic;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Entities;

namespace ClientSDK.Services;

public class GameViewModule
{
	private readonly Dictionary<EntityType, Func<IEntity, EntityView>> _creators = new();

	public ViewContainer _views = new();
	private readonly LisergySDK _client;

	public GameViewModule(LisergySDK client)
	{
		_client = client;
	}

	public event Action<EntityView>? OnViewCreated;

	public void RegisterView(EntityType t, Func<IEntity, EntityView> creator)
	{
		_creators[t] = creator;
	}

	public IEntityView GetOrCreateView(IEntity entity)
	{
		var existingView = _views.GetView(entity);
		if (existingView == null)
		{
			EntityView v;
			if (_creators.TryGetValue(entity.EntityType, out var creator))
				v = creator(entity);
			else
				v = new EntityView(entity, _client);
			_views.AddView(entity, v);
			OnViewCreated?.Invoke(v);
			return v;
		}

		return existingView;
	}

	public void Register()
	{
	}

	public IEntityView GetEntityView(IEntity entity)
	{
		return _views.GetView(entity);
	}
}