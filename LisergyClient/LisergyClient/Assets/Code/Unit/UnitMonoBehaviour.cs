using Game.Tile;
using UnityEngine;

namespace Assets.Code
{
    public class UnitMonoBehaviour : MonoBehaviour
    {
        Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void Iddle()
        {
            _anim.Play("Iddle");
        }

        public void Walking(Direction d)
        {
            _anim.Play("Running");
        }
    }
}
