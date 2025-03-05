using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Views;
using ClientSDK;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.World;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
	/// <summary>
	/// Responsible for rendering the visual representation of game paths.
	/// It listens for events for when to render the movement path on the map and events for when to hide it
	/// </summary>
	public class MovePathLogic 
	{
		private IGameClient _gameClient;
		private IDictionary<GameId, ClientMovePathView> _entityPaths = new Dictionary<GameId, ClientMovePathView>();

		public MovePathLogic(IGameClient _client)
		{
			_gameClient = _client;
		}

		private void OnCourseChanged(IEntity p)
		{
			if (_entityPaths.TryGetValue(p.EntityId, out var path))
			{
				path.Clear();
				_entityPaths.Remove(p.EntityId);
			}
		}

		public async UniTask StartMovement(EntityMovementRequestStarted ev)
		{
			var party = ev.Party;
			var path = ev.Path.Select(p => _gameClient.Game.World.GetTile(p.X, p.Y)).ToList();
			var server = _gameClient.Modules;
			var clientPath = new ClientMovePathView();
			_entityPaths[party.EntityId] = clientPath;
			for (var x = 0; x < path.Count; x++)
			{
				var nodeTile = path[x];
				var view = nodeTile.Entity.GetView();
				var hasNext = x < path.Count - 1;
				var hasPrevious = x > 0;
				if (hasNext)
				{
					var next = path[x + 1];
					var direction = nodeTile.GetDirection(next);
					await clientPath.AddPath(next.Entity.GetView<TileView>(), view.GameObject.transform.position, direction);
				}
				if (hasPrevious)
				{
					var previous = path[x - 1];
					var direction = nodeTile.GetDirection(previous);
					await clientPath.AddPath(nodeTile.Entity.GetView<TileView>(), view.GameObject.transform.position, direction);
				}
			}
		}

		public void OnFinishedMove(IEntity entity, IEntity newTile)
		{
			if (_entityPaths.ContainsKey(entity.EntityId))
			{
				var partyPath = _entityPaths[entity.EntityId];
				var pathsOnTile = partyPath.Pop(newTile);
				if (pathsOnTile != null)
					foreach (var path in pathsOnTile)
						partyPath.AddBackToPool(path);
				if (partyPath.Empty())
				{
					partyPath.Clear();
                    _entityPaths.Remove(entity.EntityId);
                }	
			}
		}
	}
}