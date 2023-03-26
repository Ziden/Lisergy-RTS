using Assets.Code.Assets.Code.Tile;
using Game;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<TileEntity>
    {
        public override TileEntity Entity { get; }
        public override GameObject GameObject { get; set; }
        public bool Decorated { get; set; }
        public override bool Instantiated => GameObject != null;

        private GameObject CloudObject;
        private PlaneMesh Mesh { get; set; }

        public TileView(TileEntity entity)
        {
            Entity = entity;
            SetCloud(true);
            if(Entity.GetNeighbor(Direction.SOUTH) == null)
            {
                AddCloud(Entity.X, Entity.Y-1);
            }
            if (Entity.GetNeighbor(Direction.NORTH) == null)
            {
                AddCloud(Entity.X, Entity.Y + 1);
            }
            if (Entity.GetNeighbor(Direction.EAST) == null)
            {
                AddCloud(Entity.X + 1, Entity.Y);
            }
            if (Entity.GetNeighbor(Direction.WEST) == null)
            {
                AddCloud(Entity.X - 1, Entity.Y);
            }
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

        private GameObject AddCloud(int x, int y)
        {
            var prefab = Resources.Load("prefabs/tiles/Cloud");
            var cloud = MainBehaviour.Instantiate(prefab) as GameObject;
            cloud.transform.position = new Vector3(x, 0.1f, y);
            return cloud;
        }

        public void SetCloud(bool clouds)
        {
            if (clouds)
            {
                CloudObject = AddCloud(Entity.X, Entity.Y);
            }
            else if (CloudObject != null)
            {
                MainBehaviour.Destroy(CloudObject);
                CloudObject = null;
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

            var chunkView = GameView.GetView<ChunkView>(Entity.Chunk);
            var parent = chunkView.GameObject.transform;
            GameObject = EntityLoader.LoadEntity(Entity, parent);
            GameObject.name = $"Tile_{Entity.X}-{Entity.Y}";
            GameObject.transform.position = new Vector3(Entity.X, 0, Entity.Y);
            var tileBhv = GameObject.GetComponent<TileMonoComponent>();
            Entity.TileId = Entity.TileId;
            tileBhv.CreateTileDecoration(this);

            foreach (var lod in GameObject.GetComponentsInChildren<LODGroup>())
            {
                lod.ForceLOD(2);
            }
            GameObject.isStatic = true;
            StaticBatchingUtility.Combine(GameObject);
            SetCloud(false);
        }


        // TODO: Remove & move calls to listeners in Entity View
        public List<WorldEntity> MovingEntities => Entity.Components.Get<TileHabitants>().EntitiesIn;
        public WorldEntity Building => Entity.Components.Get<TileHabitants>().Building;


        public override string ToString()
        {
            return $"<TileView {Entity}>";
        }
    }
}