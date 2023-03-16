using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit, IGameObject
    {
        public GameObject GameObject { get; set; }
        public Sprite3D Sprites;

        public ClientParty ClientParty { get => (ClientParty)this.Party; }

        public ClientUnit(Unit u) : base(u.SpecId)
        {
            this.Id = u.Id;
            this.Stats.SetStats(u.Stats);
            this.Name = u.Name;
        }
      
        public GameObject AddToScene()
        {
            if (GameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                Sprite[] sprites = LazyLoad.GetSprite(this.SpecId);
                GameObject = new GameObject($"Unit Spec {this.SpecId}");
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
