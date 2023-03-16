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
    public class TileView : IEntityView
    {
        public Tile Tile;

        public GameObject GameObject { get; set; }
        public bool Decorated;

        public TileView(Tile t) : base()
        {
            Tile = t;
            GameView.Events.Register<StaticEntityPlacedEvent>(this, OnStaticEntityPlaced);
            AddToScene(Tile.TileId);
        }

        private void OnStaticEntityPlaced(StaticEntityPlacedEvent ev)
        {
           
        }

        public void SetVisible(bool visible)
        {
            if (GameObject == null)
                return;

            if (visible == false)
            {
                SetColor(new Color(0.5f, 0.5f, 0.5f, 1.0f));
            }
            else
            {
                SetColor(new Color(1f, 1f, 1f, 1.0f));
                GameObject.SetActive(visible);
            }
        }

        private void SetColor(Color c)
        {
            foreach (Transform child in GameObject.transform)
            {
                var rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = c;
                }
            }
        }

        public void UpdateFrom(TileData data)
        {
            Tile.TileId = data.TileId;
            Tile.ResourceID = data.ResourceId;
            SetVisible(true);
        }

        // TODO: Remove & move calls to listeners in Entity View
        public List<WorldEntity> MovingEntities => Tile.GetComponent<EntityPlacementComponent>().EntitiesIn;
        public StaticEntity StaticEntity => Tile.GetComponent<EntityPlacementComponent>().StaticEntity;
        public void AddToScene(byte tileID)
        {
            var tileSpec = StrategyGame.Specs.Tiles[tileID];
            foreach (var art in tileSpec.Arts)
            {
                if (GameObject == null)
                {
                    var prefab = Resources.Load("prefabs/tiles/" + art.Name);
                    var chunkView = GameView.GetView<ChunkView>(Tile.Chunk);
                    var parent = chunkView.GameObject.transform;
                    GameObject = MainBehaviour.Instantiate(prefab, parent) as GameObject;
                    GameObject.name = $"Tile_{Tile.X}-{Tile.Y}";
                    GameObject.transform.position = new Vector3(Tile.X, 0, Tile.Y);
                    var tileBhv = GameObject.GetComponent<TileRandomizerBehaviour>();
                    Tile.TileId = tileID;
                    tileBhv.CreateTileDecoration(this);
                    return;
                }
            }
        }

        public override string ToString()
        {
            return $"<TileView {Tile}>";
        }
    }
}
