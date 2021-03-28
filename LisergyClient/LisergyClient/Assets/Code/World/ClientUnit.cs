using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit, IGameObject
    {
        private GameObject _gameObject;
        public Sprite3D Sprites;

        public ClientParty ClientParty { get => (ClientParty)this.Party; }

        public ClientUnit(PlayerEntity owner, Unit u) : base(u.SpecId)
        {
            this.Id = u.Id;
            this.Stats.SetStats(u.Stats);
            StackLog.Debug($"Created new unit instance {this}");
        }

        public GameObject GetGameObject() => _gameObject; 

        public GameObject Render()
        {
            if (_gameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                // TODO: Cache prefabs & Thumbnails per spec id so no duplicates
                var art = StrategyGame.Specs.Units[this.SpecId].Art;
                Sprite[] sprites = Resources.LoadAll<Sprite>("sprites/" + art.Name);
                _gameObject = new GameObject($"Unit Spec {this.SpecId}");
                var spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[0];
                Sprites = _gameObject.AddComponent<Sprite3D>();
                Sprites.Sprites = sprites;
            }
            return _gameObject;
        }
    }
}
