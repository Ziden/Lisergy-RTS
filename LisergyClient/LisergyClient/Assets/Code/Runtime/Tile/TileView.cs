using Assets.Code.Assets.Code.Tile;
using Game;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using Assets.Code.Assets.Code.Assets;
using GameAssets;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<TileEntity>
    {
        public override TileEntity Entity { get; }
        public override GameObject GameObject { get; set; }
        public bool Decorated { get; set; }
        public override bool Instantiated => GameObject != null;

        private IAssetService _assets;

        public TileView GetNeighbor(Direction d) => GameView.GetView<TileView>(Entity.GetNeighbor(d));

        public TileView(TileEntity entity)
        {
            Entity = entity;
            _assets = ServiceContainer.Resolve<IAssetService>();
            this.InitializeFog();
        }

        public void UpdateFromData(TileData data)
        {
            Entity.TileId = data.TileId;
            if (!Instantiated)
            {
                Instantiate();
                RegisterEvents();
            }
        }

        public override void Instantiate()
        {
            if (Instantiated)
            {
                return;
            }
            Entity.Components.Add(this);
            RegisterEvents();
            if (GameObject != null)
            {
                return;
            }

            var spec = Entity.GetSpec();
            var chunkView = GameView.GetView<ChunkView>(Entity.Chunk);
            var parent = chunkView.GameObject.transform;

            _assets.CreatePrefab(spec.Art, new Vector3(Entity.X, 0, Entity.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                GameObject = o;
                GameObject.transform.parent = parent;
                GameObject.name = $"Tile_{Entity.X}-{Entity.Y}";
                var tileBhv = GameObject.GetComponent<TileMonoComponent>();
                Entity.TileId = Entity.TileId;
                tileBhv.CreateTileDecoration(this);
                GameObject.isStatic = true;
                StaticBatchingUtility.Combine(GameObject);
                SetFogInTileView(false, false);
            });
        }

        public WorldEntity Building => Entity.Components.Get<TileHabitants>().Building;

        public override string ToString()
        {
            return $"<TileView {Entity}>";
        }
    }
}