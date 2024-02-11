using ClientSDK;
using DG.Tweening;
using Game;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
    /// <summary>
    /// Interface for entities who have movement interpolated
    /// </summary>
    public interface IEntityMovementInterpolated
    {
        MovementInterpolator MovementInterpolator { get; }
    }

    /// <summary>
    /// Adds a movement interpolator component to an entity and allows the entity to interpolate and predict movements.
    /// </summary>
    public class MovementInterpolator
    {
        private IGameClient _client;
        private BaseEntity _entity;
        private Tweener _currentSequence;
        private Queue<(TileEntity, TileEntity)> _queue = new Queue<(TileEntity, TileEntity)> ();

        public MovementInterpolator(IGameClient client, BaseEntity entity)
        {
            _client = client;
            _entity = entity;
        }

        /// <summary>
        /// Moves a single tile
        /// </summary>
        public void InterpolateMovement(TileEntity from, TileEntity to)
        {
            if (from == to) return;
            _client.Log.Debug($"[MovementInterpolator] Receiving interpolation request {_entity} from {from} to {to}");
            if (from.Distance(to) > 1)
            {
                _client.Log.Error($"{_entity} tried to move more than 1 tile distance using interpolation");
                return;
            }
            if (_currentSequence != null && _currentSequence.IsActive() && _currentSequence.IsPlaying())
            {
                _queue.Enqueue((from, to));
                return;
            }
            var view = _entity.GetEntityView() as IGameObject;
            var gameObject = view.GameObject;
            var moveComponent = _entity.Components.Get<MovespeedComponent>();
            var duration = moveComponent.MoveDelay.TotalSeconds;
            var tilePos = to.UnityPosition();
            var finalPos = new Vector3(tilePos.x, view.GameObject.transform.position.y, tilePos.z);
            _currentSequence = gameObject.transform.DOMove(finalPos, (float)duration).SetEase(Ease.Linear)
                .OnStart(() => OnStart(from, to))
                .OnKill(() => OnFinish(from, to))
                .SetAutoKill(true);
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        private void OnStart(TileEntity from, TileEntity to)
        {
            _client.Log.Debug($"[MovementInterpolator] Interpolation Started {_entity} from {from} to {to}");
            _client.ClientEvents.Call(new MovementInterpolationStart()
            {
                Entity = _entity,
                From = from,
                To = to
            });
        }

        private void OnFinish(TileEntity from, TileEntity to)
        {
            _currentSequence = null;
            _client.ClientEvents.Call(new MovementInterpolationEnd()
            {
                Entity = _entity,
                LastStep = _queue.Count == 0,
                From = from,
                To = to,
                
            });
            if (_queue.TryDequeue(out var newMove))
            {
                InterpolateMovement(newMove.Item1, newMove.Item2);
            }
        }
    }
}
