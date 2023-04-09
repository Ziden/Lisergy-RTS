using Game;
using Game.Tile;
using UnityEngine;

namespace Assets.Code
{
    public class UnitMonoBehaviour : MonoBehaviour
    {
        private Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void AnimIddle()
        {
            Log.Debug("Anim Iddle");
            _anim.Play("Iddle");
        }

        public void AnimWalking()
        {
            Log.Debug("Anim Walk");
            _anim.Play("Running");
        }
    }
}
