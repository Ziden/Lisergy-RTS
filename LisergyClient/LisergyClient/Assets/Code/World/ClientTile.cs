using Game;
using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientTile : Tile
    {
        public GameObject GameObj { get => _gameObj; private set => _gameObj = value; }

        private GameObject _gameObj;

        public bool Decorated;

        public ClientWorld ClientWorld { get => (ClientWorld)this.World; }

        public ClientTile(ClientChunk c, int x, int y) : base(c, x, y) { }

        public void UpdateVisibility()
        {
            if (GameObj == null)
                return;

            var isVisible = IsVisibleTo(MainBehaviour.Player);
            if (GameObj.activeSelf == isVisible)
                return;

            StackLog.Debug($"Changing activaction of {this} to {isVisible}");
            //GameObj.SetActive(isVisible); // mandando unit parece q caga isso
        }

        public override void SetSeenBy(ExploringEntity entity)
        {
            base.SetSeenBy(entity);
            StackLog.Debug($"{entity} sees {this} client {MainBehaviour.Player.UserID}");
            UpdateVisibility();
        }

        public void RenderTile(byte tileID)
        {
            var tileSpec = StrategyGame.Specs.GetTileSpec(tileID);
            foreach (var art in tileSpec.Arts)
            {
                if (GameObj == null)
                {
                    var prefab = Resources.Load("prefabs/tiles/" + art.Name);
                    var parent = ((ClientChunk)this.Chunk).ChunkObject.transform;
                    GameObj = MainBehaviour.Instantiate(prefab, parent) as GameObject;
                    GameObj.name = $"Tile_{X}-{Y}";
                    GameObj.transform.position = new Vector3(X, 0, Y);
                    var tileBhv = GameObj.GetComponent<TileRandomizerBehaviour>();
                    base.TileId = tileID;
                    tileBhv.CreateTileDecoration(this);
                    return;
                }
            }
        }

        public override byte TileId
        {
            get { return base.TileId; }
            set
            {
                StackLog.Debug($"Updating {this} tileid to {value}");
                RenderTile(value);
                base.TileId = value;
            }
        }

        public override Building Building
        {
            get { return base.Building; }
            set
            {
                if (value != null)
                {
                    var clientBuilding = value as ClientBuilding;
                    clientBuilding.Object.transform.position = new Vector3(X, 0, Y);
                }
                using (new StackLog($"[Building] New {value} on {this}"))
                {
                    base.Building = value;
                }
            }
        }
    }
}
