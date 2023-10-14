using Game.Tile;
using Game.World;
using Assets.Code.Assets.Code.Assets;
using Game.Systems.Tile;
using ClientSDK.Data;
using UnityEngine;
using Game;
using System.Collections.Generic;
using Game.DataTypes;

namespace Assets.Code.Views
{
    public partial class TileView : UnityEntityView<TileEntity>
    {
        public bool Decorated { get; set; }

        private static Dictionary<GameId, GameObject> _chunks = new Dictionary<GameId, GameObject>();

        public IEntityView GetNeighbor(Direction d) => Client.Modules.Views.GetOrCreate<TileEntity>(Entity.GetNeighbor(d));

        private IAssetService _assets;

        protected override void CreateView()
        {
            _assets = ServiceContainer.Resolve<IAssetService>();

            var spec = Client.Game.Specs.Tiles[Entity.SpecId];
          

            _assets.CreatePrefab(spec.Art, new Vector3(Entity.X, 0, Entity.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                var chunk = Entity.Chunk;
                if(!_chunks.TryGetValue(chunk.EntityId, out var chunkObject))
                {
                    Log.Debug($"Registered chunk {chunk.EntityId}");
                    chunkObject = new GameObject($"Chunk {chunk.X}-{chunk.Y}");
                    _chunks[chunk.EntityId] = chunkObject;
                    chunkObject.transform.parent = ViewContainer.transform;
                    chunkObject.isStatic = true;
                }
              
                o.transform.parent = chunkObject.transform;
                o.name = $"Tile_{Entity.X}-{Entity.Y}";
                GameObject = o;
                InitializeFog();
                var tileBhv = o.GetComponent<TileMonoComponent>();
                tileBhv.CreateTileDecoration(this);
                o.isStatic = true;
                //StaticBatchingUtility.Combine(chunkObject);
                State = EntityViewState.RENDERED;
                Log.Debug($"Tile {Entity.X}-{Entity.Y} instantiated");
            });
        }

        public override string ToString() => $"<TileView {Entity}>";
    }
}