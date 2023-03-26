using Game;
using Game.Tile;
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

        private PlaneMesh Mesh { get; set; }

        public TileView(TileEntity entity)
        {
            Entity = entity;
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
            if(Instantiated)
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
            var tileBhv = GameObject.GetComponent<TileRandomizerBehaviour>();
            Entity.TileId = Entity.TileId;
            tileBhv.CreateTileDecoration(this);

            foreach (var lod in GameObject.GetComponentsInChildren<LODGroup>())
            {
                lod.ForceLOD(2);
            }
            GameObject.isStatic = true;
            StaticBatchingUtility.Combine(GameObject);
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