using Assets.Code.Assets.Code.Runtime.Tools;
using Assets.Code.World;
using DG.Tweening;
using Game;
using Game.Battle;
using Game.BattleActions;
using Game.BattleEvents;
using Game.Battler;
using Game.DataTypes;
using Game.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Code.Battle
{
    public partial class BattlePlayback
    {
        private async Task PlayEvent(BattleEvent ev)
        {
            if (ev is UnitDeadEvent unitDead)
            {
                var unitView = _units[unitDead.UnitId];
                unitView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.Death);
            }
            else if (ev is AttackAction atk)
            {
                atk.Battle = _battle;
                var result = atk.Result as AttackActionResult;
                var attackerView = _units[atk.Unit.UnitID];
                var defenderView = _units[atk.Defender.UnitID];

                var ogPosition = attackerView.GameObject.transform.position;

                // Attack sequence
                var sequence = DOTween.Sequence();
                // Move in front of enemy
                var dest = defenderView.GameObject.transform.position;
                var n = (defenderView.GameObject.transform.position - attackerView.GameObject.transform.position).normalized / 2.5f;
                if (dest.x > attackerView.GameObject.transform.position.x) dest -= n;
                else if (dest.x < attackerView.GameObject.transform.position.x) dest -= n;

                _screen.ToggleBar(atk.UnitID, false);

                sequence.Append(
                    attackerView.GameObject.transform.DOMove(dest, 0.8f)
                    .OnStart(() => attackerView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.Jump))
                    .OnComplete(() => attackerView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.MeleeAttack)));
                sequence.AppendInterval(0.3f);

                // Hit Effect and damage
                sequence.AppendCallback(() =>
                {
                    defenderView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.Damaged, 0.7f);
                    ShowDamage(defenderView, result.Damage);
                    OnAttacked?.Invoke(atk);
                });
                sequence.AppendInterval(0.6f);

                // Go back
                sequence.Append(
                   attackerView.GameObject.transform.DOMove(ogPosition, 0.6f)
                   .OnStart(() =>
                   {
                       attackerView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.JumpBack);
                       defenderView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.BattleIddle);
                   }));
                sequence.Play();
                while (sequence.IsPlaying()) await Task.Delay(1);
                _screen.ToggleBar(atk.UnitID, true);
                attackerView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.BattleIddle);
            }
        }
    }
}

