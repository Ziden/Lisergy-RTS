using Assets.Code.Views;
using Game;
using Game.ECS;
using UnityEngine;

namespace Assets.Code.World
{
    public class UnitView : IGameObject
    {
        public GameObject GameObject { get; set; }
        public Unit Unit;
        public Sprite3D Sprites;

        public UnitView(Unit unit)
        {
            Unit = unit;
        }

        public GameObject AddToScene()
        {
            if (GameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                Sprite[] sprites = LazyLoad.GetSprite(Unit.SpecId);
                GameObject = new GameObject($"Unit Spec {Unit.SpecId}");
                var spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[0];
                Sprites = GameObject.AddComponent<Sprite3D>();
                Sprites.Sprites = sprites;
                GameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            return GameObject;
        }

    }
}
