using Game.Entities;
using Game.Tile;
using UnityEngine;

namespace Assets.Code
{
    // Refers to animation ID in unit Animator configuration
    public enum UnitAnimation
    {
        Iddle = 0, 
        Running = 1, 
        Dange = 2, 
        MeleeAttack = 3, 
        BattleIddle = 4, 
        Damaged = 5,
        Jump = 6,
        JumpBack = 7,
        Death = 8
    }

    /// <summary>
    /// Can be added to units to control animation
    /// TODO: Move to UnitView
    /// </summary>
    public class UnitBehaviour : MonoBehaviour
    {
        private Animator _anim;
        private string _current;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void PlayAnimation(UnitAnimation anim, float speed = 1f)
        {
            _anim.speed = speed;
            if(_current != null) _anim.ResetTrigger(_current);
            _current = anim.ToString();
            _anim.SetTrigger(anim.ToString());
        }

        public void SetAnimSpeed(float speed)
        {
            _anim.speed = speed;
        }
    }
}
