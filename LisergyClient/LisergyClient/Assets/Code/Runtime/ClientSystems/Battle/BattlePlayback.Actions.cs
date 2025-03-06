﻿using DG.Tweening;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.BattleEvents;
using Game.Systems.Battle.Data;
using GameAssets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Code.Battle
{
    public partial class BattlePlayback
    {
        public event Action<AttackAction> OnAttacked;
        public event Action<UnitDeadEvent> OnUnitDied;
        public event Action<BattleUnit> OnUnitAct;
        public event Action<BattleUnit> OnUnitFinishAct;
        public event Action OnBattleFinish;

        private async Task PlayEvent(BattleEvent ev)
        {
            if (ev is UnitDeadEvent unitDead)
            {
                var unitView = Units[unitDead.UnitId];
                unitView.Animations.PlayAnimation(UnitAnimation.Death);
                _audio.PlaySoundEffect(SoundFX.Ogre3);
            }
            else if (ev is AttackAction atk)
            {
                atk.Battle = _battle;
                var result = atk.Result as AttackActionResult;
                var attackerView = Units[atk.Unit.UnitID];
                var defenderView = Units[atk.Defender.UnitID];

                var ogPosition = attackerView.GameObject.transform.position;

                // Attack sequence
                var sequence = DOTween.Sequence();
                // Move in front of enemy
                var dest = defenderView.GameObject.transform.position;
                var n = (defenderView.GameObject.transform.position - attackerView.GameObject.transform.position).normalized / 2.1f;
                if (dest.x > attackerView.GameObject.transform.position.x) dest -= n;
                else if (dest.x < attackerView.GameObject.transform.position.x) dest -= n;

                OnUnitAct?.Invoke(atk.Unit);

                sequence.Append(
                    attackerView.GameObject.transform.DOMove(dest, 0.5f)
                    .OnStart(() => attackerView.Animations.PlayAnimation(UnitAnimation.Running))
                    .OnComplete(() =>
                    {
                        _audio.PlaySoundEffect(SoundFX.Swing);
                        attackerView.Animations.PlayAnimation(UnitAnimation.MeleeAttack);

                    }));
                sequence.AppendInterval(0.3f);

                // Hit Effect and damage
                sequence.AppendCallback(() =>
                {
                    _audio.PlaySoundEffect(SoundFX.Sword_unsheathe5);
                    defenderView.Animations.PlayAnimation(UnitAnimation.Damaged, 0.7f);
                    ShowDamage(defenderView, result.Damage);
                    OnAttacked?.Invoke(atk);
                });
                sequence.AppendInterval(0.55f);

                // Go back
                sequence.Append(
                   attackerView.GameObject.transform.DOMove(ogPosition, 0.3f)
                   .OnStart(() =>
                   {
                       attackerView.Animations.PlayAnimation(UnitAnimation.JumpBack);
                       defenderView.Animations.PlayAnimation(UnitAnimation.BattleIddle);
                   }));
                sequence.Play();
                while (sequence.IsPlaying()) await Task.Delay(1);
                OnUnitFinishAct?.Invoke(atk.Unit);
                attackerView.Animations.PlayAnimation(UnitAnimation.BattleIddle);
            }
        }
    }
}

