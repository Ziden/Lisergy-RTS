using Assets.Code.Code.Utils;
using Assets.Code.Views;
using DG.Tweening;
using Game;
using Game.Systems.Movement;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
    /// <summary>
    /// Adds a movement interpolator component to an entity and allows the entity to interpolate and predict movements.
    /// </summary>
    public class MovementInterpolator
    {
        public event TweenCallback OnStart;
        public event TweenCallback OnFinish;
        public event Action<TileView> OnPrepareMoveNext;

        private BaseEntity _entity;
        private ClientMovementInterpolationComponent _component;

        public MovementInterpolator(BaseEntity entity)
        {
            _entity = entity;
            _component = entity.Components.AddReference(new ClientMovementInterpolationComponent());
        }

        public bool Running => _component.InterpolingPath?.Count > 0;

        public TileEntity Next => _component.InterpolingPath[0];

        /// <summary>
        /// When server confirms a move.
        /// Will return if the predicted interpolation desynched or not.
        /// </summary>
        public bool Confirmed(TileEntity confirmedTile)
        {
            Debug.Log($"Confirming {confirmedTile}");
            if (Running)
            {
                if (Next == confirmedTile)
                {
                    _component.InterpolingPath.RemoveAt(0);
                    return true;
                }
                else
                {
                    if (TryResync(confirmedTile)) return true;
                    _component.TweenPath?.Kill();
                    _component.InterpolingPath = null;

                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to resync the interpolation in case we received a wrong confirmation
        /// </summary>
        private bool TryResync(TileEntity confirmedTile)
        {
            var index = _component.InterpolingPath.IndexOf(confirmedTile);
            if (index != -1)
            {
                Debug.LogWarning("Resync Movement");
                _component.InterpolingPath.RemoveRange(0, index);
                _component.TweenPath.GotoWaypoint(index);
                return true;
            }
            return false;
        }

        public void Start(List<TileEntity> tiles)
        {
            if (Running)
            {
                _component.TweenPath.Kill();
                _component.InterpolingPath.Clear();
            }

            var view = GameView.GetView(_entity);
            var gameObject = view.GameObject;
            var moveComponent = _entity.Components.Get<CourseComponent>();
            var duration = (moveComponent.MoveDelay.TotalSeconds + MainBehaviour.Networking.Delta) * tiles.Count;
            var y = gameObject.transform.position.y;
            _component.InterpolingPath = new List<TileEntity>(tiles);
            _component.InterpolingPath.RemoveAt(0);

            _component.TweenPath = gameObject.transform.DOPath(
                _component.InterpolingPath.Select(t => t.Position(y)).ToArray(),
                (float)duration,
                PathType.CatmullRom,
                PathMode.TopDown2D
            );
            _component.TweenPath.OnStart(OnStart);
            _component.TweenPath.SetEase(Ease.Linear);
            _component.TweenPath.OnWaypointChange(idx => OnPrepareMoveNext(GameView.GetView<TileView>(_component.InterpolingPath[idx])));
            _component.TweenPath.onComplete += () =>
            {
                _component.TweenPath = null;
            };
            _component.TweenPath.SetAutoKill(true);
            _component.TweenPath.OnKill(OnFinish);
        }
    }
}
