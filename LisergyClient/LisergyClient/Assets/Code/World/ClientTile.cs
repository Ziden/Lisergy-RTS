using Assets.Code.Art;
using Game;
using Game.World;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientTile : Tile
    {
        private GameObject _rootNode;
        private GameObject _plane;

        public ClientTile(ClientChunk c, int x, int y) : base(c, x, y)
        {
            Draw();
        }

        public override byte TileId
        {
            get { return base.TileId; }
            set
            {
                Log.Debug($"Updating {this} tileid to {value}");
                var tileSpec = StrategyGame.Specs.GetTileSpec(value);
                foreach(var art in tileSpec.Arts)
                {
                    if(art.Type==GameData.Specs.ArtType.SPRITE)
                        TextureManager.SetTexture(_plane, art.Name);
                    else if (art.Type == GameData.Specs.ArtType.PREFAB)
                    {
                        var prefab = Resources.Load("prefabs/tiles/"+ art.Name);
                        var obj = MainBehaviour.Instantiate(prefab) as GameObject;
                        obj.transform.SetParent(_rootNode.transform);
                        obj.transform.position = new Vector3(X, 0, Y);
                    }
                }
                base.TileId = value;
            }
        }

        public override Building Building
        {
            get
            {
                return base.Building;
            }
            set
            {
                if (value != null)
                {
                    var clientBuilding = value as ClientBuilding;
                    clientBuilding.Object.transform.position = new Vector3(X, 0, Y);
                }
                base.Building = value;
            }
        }

        public void Draw()
        {
            var chunk = (ClientChunk)this.Chunk;
            _rootNode = new GameObject($"Tile-{X}-{Y}");

            _rootNode.transform.SetParent(chunk.ChunkObject.transform);

            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _plane.transform.SetParent(_rootNode.transform);
            _plane.transform.localScale = new Vector3(0.1f, 1, 0.1f);
            _plane.transform.position = new Vector3(0, 0, 0);
            _rootNode.transform.position = new Vector3(X, 0, Y);
        }
    }
}
