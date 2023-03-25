using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Code.Utils;
using Assets.Code.Entity;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Game;
using Game.Battler;
using Game.DataTypes;
using Game.Events.GameEvents;
using Game.Movement;
using Game.Party;
using Game.Tile;
using UnityEngine;

namespace Assets.Code.World
{
    public partial class PartyView
    {
        private TweenerCore<Vector3, Path, PathOptions> path;
        private bool interpoling = false;
        private List<TileEntity> interpolingPath;

        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<PartyView, EntityMoveInEvent>(OnMoveIn);
            Entity.BattleGroupLogic.OnBattleIdChanged += OnBattleIdChanged;
            ClientEvents.OnStartMovementRequest += OnPartyMovementStarted;
        }

        private void OnPartyMovementStarted(PartyEntity party, List<TileEntity> tiles)
        {
            if (party == this.Entity)
            {
                var moveComponent = Entity.Components.Get<EntityMovementComponent>();
                Debug.Log("MoveDelay: " + moveComponent.MoveDelay.TotalSeconds);
                var duration = (moveComponent.MoveDelay.TotalSeconds + MainBehaviour.Networking.Delta) * tiles.Count;
                var y = GameObject.transform.position.y;
                interpoling = true;
                interpolingPath = new List<TileEntity>(tiles);
                // Remove the first one that is the player current position
                interpolingPath.RemoveAt(0);

                path = GameObject.transform.DOPath(
                    interpolingPath.Select(t => t.Position(y)).ToArray(),
                    (float)duration,
                    PathType.Linear,
                    PathMode.Ignore
                );
                path.SetEase(Ease.Linear);
                path.SetAutoKill(true);
                path.onComplete += () =>
                {
                    interpoling = false;
                    path = null;
                };
            }
        }

        private void OnBattleIdChanged(IBattleComponentsLogic logic, GameId battleId)
        {
            if (battleId.IsZero()) EntityEffects<PartyEntity>.StopEffects(Entity);
            else EntityEffects<PartyEntity>.BattleEffect(Entity);
        }

        private static void OnMoveIn(PartyView view, EntityMoveInEvent ev)
        {
            if (view.Entity.IsMine())
            {
                ClientEvents.PartyFinishedMove(view.Entity, ev.FromTile, ev.ToTile);
            }
        }


        public void Move(EntityMoveInEvent ev)
        {
            // Receing the tiles from the interpoling task
            if (interpolingPath?.Count > 0)
            {
                var first = interpolingPath[0];
                if (first == ev.ToTile)
                {
                    interpolingPath.RemoveAt(0);
                    if (interpolingPath.Count == 0)
                    {
                        Debug.Log("Finished interpolation of " + Entity.Id);
                    }

                    return;
                }
                // Some desynch happend, so the old path is no longer valid
                path?.Kill();
                interpolingPath = null;
            }

            GameObject.transform.position = ev.ToTile.Position(GameObject.transform.position.y);
        }
    }
}