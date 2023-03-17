using Assets.Code.World;
using Game;
using Game.Events.GameEvents;
using Game.World.Components;
using Game.World.Data;
using System;
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

        public TileView(Tile t) : base()
        {
            Entity = t;
        }

        public void UpdateFrom(TileData data)
        {
            Entity.TileId = data.TileId;
            Entity.ResourceID = data.ResourceId;
            if(!Instantiated)
            {
                Instantiate();
                SetFogOfWar(true);
            }
      
        }

        public override void Instantiate()
        {
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
        public List<WorldEntity> MovingEntities => Entity.GetComponent<EntityPlacementComponent>().EntitiesIn;
        public StaticEntity StaticEntity => Entity.GetComponent<EntityPlacementComponent>().StaticEntity;


        public override string ToString()
        {
            return $"<TileView {Entity}>";
        }
    }
}
