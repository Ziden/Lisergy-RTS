using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit, IGameObject
    {
        private GameObject _gameObject;
        public Sprite3D Sprites;

        public ClientParty ClientParty { get => (ClientParty)this.Party; }

        public ClientUnit(Unit u) : base(u.SpecId)
        {
            this.Id = u.Id;
            this.Stats.SetStats(u.Stats);
            this.Name = u.Name;
        }
      
        public GameObject GetGameObject() => _gameObject; 

        public GameObject AddToScene()
        {
            if (_gameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                Sprite[] sprites = LazyLoad.GetSprite(this.SpecId);
                _gameObject = new GameObject($"Unit Spec {this.SpecId}");
                var spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[0];
                Sprites = _gameObject.AddComponent<Sprite3D>();
                Sprites.Sprites = sprites;
                _gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            return _gameObject;
        }    
    }
}
