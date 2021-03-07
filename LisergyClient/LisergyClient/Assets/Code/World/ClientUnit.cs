using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit
    {
        public GameObject GameObject;
        public Sprite3D Sprites;

        public ClientParty ClientParty { get => (ClientParty)this.Party; }

        public ClientUnit(PlayerEntity owner, Unit u) : base(u.SpecId)
        {
            StackLog.Debug($"Created new unit instance {this}");
        }

        public GameObject Render()
        {
            if (GameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                // TODO: Cache prefabs & Thumbnails so no duplicates
                var art = StrategyGame.Specs.Units[this.SpecId].Art;
                Sprite[] sprites = Resources.LoadAll<Sprite>("sprites/" + art.Name);
                GameObject = new GameObject($"Unit Spec {this.SpecId}");
                var spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[0];
                Sprites = GameObject.AddComponent<Sprite3D>();
                Sprites.Sprites = sprites;
            }
            return GameObject;
        }
    }
}
