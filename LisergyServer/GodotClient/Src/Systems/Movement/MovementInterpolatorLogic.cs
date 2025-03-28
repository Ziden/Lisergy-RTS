using ClientSDK;
using Game.Engine.ECLS;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;
using Godot;
using System;
using System.Collections.Generic;

namespace LisergyGodotClient.Src.Systems.Movement
{
    /// <summary>
    /// Interface for entities who have movement interpolated
    /// </summary>
    public interface IEntityMovementInterpolated
    {
        MovementInterpolatorLogic MovementInterpolator { get; }
    }

    /// <summary>
    /// Adds a movement interpolator component to an entity and allows the entity to interpolate and predict movements.
    /// </summary>
    public class MovementInterpolatorLogic
    {
        private IClientSDK _client;
        private IEntity _entity;
        private Tween _currentTween;
        private Queue<(TileModel, TileModel)> _queue = new Queue<(TileModel, TileModel)>();

        public bool IsInterpolating()
        {
            return _currentTween != null && _currentTween.IsRunning();
        }

        public bool HasQueue() => _queue.Count > 0;

        public MovementInterpolatorLogic(IClientSDK client, IEntity entity)
        {
            _client = client;
            _entity = entity;
        }

        /// <summary>
        /// Moves a single tile
        /// </summary>
        public void InterpolateMovement(TileModel from, TileModel to)
        {
            if (from == to) return;
            _client.Log.Debug($"[MovementInterpolator] Receiving interpolation request {_entity} from {from} to {to}");
            if (from.Distance(to) > 1)
            {
                _client.Log.Error($"{_entity} tried to move more than 1 tile distance using interpolation");
                return;
            }
            if (IsInterpolating())
            {
                _queue.Enqueue((from, to));
                return;
            }
            var view = _entity.GetView();
            var gameObject = view.GameObject;
            var moveComponent = _entity.Components.Get<MovespeedComponent>();
            var duration = moveComponent.MoveDelay.TotalSeconds;

            Node3D node3D = gameObject.Get<Node3D>();
            if (node3D == null)
            {
                _client.Log.Error($"[MovementInterpolator] GameObject doesn't contain a Node3D: {gameObject.Name}");
                return;
            }

            Vector3 tilePos = to.Position.ToGodotVector3();
            Vector3 finalPos = new Vector3(tilePos.X, node3D.GlobalPosition.Y, tilePos.Z);
            if (_currentTween != null && _currentTween.IsValid())
            {
                _currentTween.Kill();
            }

            _currentTween = node3D.CreateTween();
            _currentTween.SetEase(Tween.EaseType.InOut);
            _currentTween.TweenProperty(node3D, "global_position", finalPos, (float)duration);
            _currentTween.Finished += () => OnFinish(from, to);
            _currentTween.Play();
            OnStart(from, to);
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }

        private void OnStart(TileModel from, TileModel to)
        {
            _client.Log.Debug($"[MovementInterpolator] Interpolation Started {_entity} from {from} to {to}");
            _client.ClientEvents.Call(new MovementInterpolationStartEvent()
            {
                Entity = _entity,
                From = from,
                To = to
            });
        }

        private void OnFinish(TileModel from, TileModel to)
        {
            _currentTween = null;
            _client.ClientEvents.Call(new MovementInterpolationEndEvent()
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
