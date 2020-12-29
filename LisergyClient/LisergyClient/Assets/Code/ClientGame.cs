using Assets.Code.World;
using Game;
using GameData;
using UnityEngine;

namespace Assets.Code
{
    public class ClientStrategyGame : StrategyGame
    {
        private Ray ray;
        private RaycastHit hit;
        private GameObject _cursor;

        public ClientStrategyGame(GameConfiguration cfg, GameSpec specs, GameWorld world) : base(cfg, specs, world) { }

        public ClientWorld GetWorld()
        {
            return World as ClientWorld;
        }

        public GameObject Cursor
        {
            get
            {
                if (_cursor == null)
                {
                    var prefab = Resources.Load("prefabs/tiles/Cursor");
                    _cursor = MainBehaviour.Instantiate(prefab) as GameObject;
                }
                return _cursor;
            }
            set => _cursor = value;
        }

        public void Update()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null)
                    return;
                var tileComponent = hit.collider.GetComponentInParent<TileRandomizerBehaviour>();
                if (tileComponent == null)
                    return;
                Cursor.transform.position = new Vector3(tileComponent.Tile.X, 0, tileComponent.Tile.Y);
            }
        }
    }
}
