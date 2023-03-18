using Assets.Code.World;
using Game;
using Game.Events.GameEvents;
using Game.World.Components;
using Game.World.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Views
{
    public partial class TileView : EntityView<Tile>
    {
        public override Tile Entity { get; }
        public override GameObject GameObject { get; set; }
        public bool Decorated { get; set; }
        public override bool Instantiated => GameObject != null;

        public TileView(Tile entity)
        {
            Entity = entity;
            Entity.Components.AddComponent(this);
        }

        public void UpdateFrom(TileData data)
        {
            Entity.TileId = data.TileId;
            if (!Instantiated)
            {
                Instantiate();
                SetFogOfWarDisabled(true);
                RegisterEvents();
            }
        }

        public void SetFogOfWarDisabled(bool isTileInLos)
        {
            if (!Instantiated)
                return;

            if (isTileInLos == false)
            {
                SetColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            }
            else
            {
                SetColor(new Color(1f, 1f, 1f, 1.0f));
                GameObject.SetActive(isTileInLos);
            }
        }

        private void SetColor(Color c)
        {
            foreach (var r in GameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.color = c;
            }
        }

        public override void Instantiate()
        {
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
        }


        // TODO: Remove & move calls to listeners in Entity View
        public List<WorldEntity> MovingEntities => Entity.GetComponent<EntityPlacementComponent>().EntitiesIn;
        public StaticEntity StaticEntity => Entity.GetComponent<EntityPlacementComponent>().StaticEntity;


        public override string ToString()
        {
            return $"<TileView {Entity}>";
        }
    }
}