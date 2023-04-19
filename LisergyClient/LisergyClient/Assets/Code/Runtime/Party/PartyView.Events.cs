using System.Collections.Generic;
using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Code.Utils;
using Assets.Code.Entity;
using Assets.Code.Views;
using DG.Tweening;
using Game.Battler;
using Game.DataTypes;
using Game.Events.GameEvents;
using Game.Party;
using Game.Tile;

namespace Assets.Code.World
{
    public partial class PartyView
    {
        private MovementInterpolator _interpolation;

        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<PartyView, EntityMoveInEvent>(OnMoveIn);
            Entity.BattleGroupLogic.OnBattleIdChanged += OnBattleIdChanged;
            _interpolation.OnStart += OnMovementStart;
            _interpolation.OnFinish += OnMovementStop;
            _interpolation.OnPrepareMoveNext += OnMovementPrepareNext;
            ClientEvents.OnStartMovementRequest += OnPartyMovementStarted;
        }

        private void OnMovementStart()
        {
            foreach (var unit in _unitObjects.Values)
            {
                unit.UnitMonoBehaviour.PlayAnimation(UnitAnimation.Running);
            }
        }

        private void OnMovementStop()
        {
            foreach (var unit in _unitObjects.Values)
            {
                unit.UnitMonoBehaviour.PlayAnimation(UnitAnimation.Iddle);
            }
        }

        private void OnMovementPrepareNext(TileView next)
        {
            foreach (var unit in _unitObjects.Values)
            {
                var lookPos = next.GameObject.transform.position - unit.GameObject.transform.position;
                lookPos.y = 0;
                unit.GameObject.transform.DOLookAt(next.GameObject.transform.position, 0.5f, AxisConstraint.Y);
            }
        }

        private void OnPartyMovementStarted(PartyEntity party, List<TileEntity> tiles)
        {
            if (party == this.Entity)
            {
                _interpolation.Start(tiles);
            }
        }

        private void OnBattleIdChanged(IBattleComponentsLogic logic, GameId battleId)
        {
            if (battleId.IsZero()) EntityEffects<PartyEntity>.StopEffects(Entity);
            else EntityEffects<PartyEntity>.BattleEffect(Entity);
        }

        private static void OnMoveIn(PartyView view, EntityMoveInEvent ev)
        {
            if (view.NeedsInstantiate) return;

            if (view.Entity.IsMine())
            {
                ClientEvents.PartyFinishedMove(view.Entity, ev.FromTile, ev.ToTile);
            }
            if (!view._interpolation.Confirmed(ev.ToTile))
            {
                view.GameObject.transform.position = ev.ToTile.Position(view.GameObject.transform.position.y);
            }
        }
    }
}