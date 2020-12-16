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
                TextureManager.SetTexture(_plane, tileSpec.Art.SpriteName);
                base.TileId = value;
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
            TextureManager.SetTexture(_plane, "planks_oak");
        }
    }
}
