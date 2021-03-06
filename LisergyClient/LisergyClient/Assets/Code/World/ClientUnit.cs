using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit
    {
        private static GameObject _UNITS_NODE;

        public GameObject GameObject;
        public Sprite3D Sprites;

        public ClientParty ClientParty { get => (ClientParty)this.Party; }

        public ClientUnit(PlayerEntity owner, Unit u) : base(u.SpecID)
        {
            StackLog.Debug($"Created new unit instance {this}");
        }

        private static GameObject UnitsContainerNode
        {
            get
            {
                if (_UNITS_NODE == null)
                    _UNITS_NODE = new GameObject("Units Container");
                return _UNITS_NODE;
            }
        }

        public void Render(int X, int Y)
        {
            if (GameObject == null)
            {
                // TODO: Cache prefabs & Thumbnails so no duplicates
                var art = StrategyGame.Specs.Units[this.SpecID].Art;
                Sprite[] sprites = Resources.LoadAll<Sprite>("sprites/" + art.Name);
                GameObject = new GameObject($"Unit Spec {this.SpecID}");
                // TODO: Parent not working
                GameObject.transform.SetParent(UnitsContainerNode.transform);
                var spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[0];
                GameObject.transform.position = new Vector3(X, 0.1f, Y);
                Sprites = GameObject.AddComponent<Sprite3D>();
                Sprites.Sprites = sprites;
            }
        }
    }
}
