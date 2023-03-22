using System.Collections.Generic;
using System.Linq;
using Assets.Code.Code.Utils;
using Assets.Code.Entity;
using DG.Tweening;
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
        private bool interpoling = false;

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
                GameObject.transform.DOPath(
                    tiles.Select(t => t.Position(y)).ToArray(),
                    (float)duration,
                    PathType.Linear,
                    PathMode.Ignore
                ).onComplete += () => { interpoling = false; };
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
            // TODO Rollback
            
        }
    }
}