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
        }

        public void UpdateFrom(TileData data)
        {
            Entity.TileId = data.TileId;
            Instantiate();
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
            if(Instantiated)
            {
                return;
            }
            SetFogOfWarDisabled(true);
            Entity.Components.Add(this);
            RegisterEvents();
            var tileSpec = StrategyGame.Specs.Tiles[Entity.TileId];
            foreach (var art in tileSpec.Arts)
            {
                if (GameObject == null)
                {
                    var prefab = Resources.Load("prefabs/tiles/" + art.Name);
                    var chunkView = GameView.GetView<ChunkView>(Entity.Chunk);
                    var parent = chunkView.GameObject.transform;
                    GameObject = MainBehaviour.Instantiate(prefab, parent) as GameObject;
                    GameObject.name = $"Tile_{Entity.X}-{Entity.Y}";
                    GameObject.transform.position = new Vector3(Entity.X, 0, Entity.Y);
                    var tileBhv = GameObject.GetComponent<TileRandomizerBehaviour>();
                    Entity.TileId = Entity.TileId;
                    tileBhv.CreateTileDecoration(this);
                    return;
                }
            }
        }


        // TODO: Remove & move calls to listeners in Entity View
        public List<WorldEntity> MovingEntities => Entity.Components.Get<EntityPlacementComponent>().EntitiesIn;
        public StaticEntity StaticEntity => Entity.Components.Get<EntityPlacementComponent>().StaticEntity;


        public override string ToString()
        {
            return $"<TileView {Entity}>";
        }
    }
}
