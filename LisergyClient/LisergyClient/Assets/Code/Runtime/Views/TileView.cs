using Game.Tile;
using Game.World;
using Assets.Code.Assets.Code.Assets;
using Game.Systems.Tile;
using ClientSDK.Data;
using UnityEngine;
using Game;
using System.Collections.Generic;
using Game.DataTypes;
using GameAssets;

namespace Assets.Code.Views
{
    public partial class TileView : UnityEntityView<TileEntity>
    {
        private static Dictionary<GameId, GameObject> _chunks = new Dictionary<GameId, GameObject>();
        private static GameObject FogContainer => _fogContainer = _fogContainer ?? new GameObject("Fog Container");
        private static GameObject _fogContainer;
        public GameObject FogObject { get; private set; } = null;
        public FogState FogState { get; private set; } = FogState.UNEXPLORED;
        public bool Decorated { get; set; }

        protected override void CreateView()
        {
            var tileSpec = Client.Game.Specs.Tiles[Entity.SpecId];
            Assets.CreatePrefab(tileSpec.Art, new Vector3(Entity.X, 0, Entity.Y), Quaternion.identity, o =>
            {
                GameObject = o;
                GameObject.transform.parent = GetChunkObject().transform;
                GameObject.name = $"Tile_{Entity.X}-{Entity.Y}";
                GameObject.GetComponent<TileMonoComponent>().CreateTileDecoration(this);
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
                Client.ClientEvents.Call(new TileViewRendered() { View = this });
            });
        }

        /// <summary>
        /// We divide the tiles in the world in small chunks
        /// We do this by attaching all child tiles of a given chunk to a parent chunk transform
        /// </summary>
        private GameObject GetChunkObject()
        {
            var chunk = Entity.Chunk;
            if (!_chunks.TryGetValue(chunk.EntityId, out var chunkObject))
            {
                Log.Debug($"Registered chunk {chunk.EntityId}");
                chunkObject = new GameObject($"Chunk {chunk.X}-{chunk.Y}");
                _chunks[chunk.EntityId] = chunkObject;
                chunkObject.transform.parent = ViewContainer.transform;
                chunkObject.isStatic = true;
            }
            return chunkObject;
        }

        public void SetFogState(FogState state)
        {
            FogState = state;
            if (state == FogState.EXPLORED)
            {
                if (FogObject != null)
                {
                    GameObject.Destroy(FogObject);
                    FogObject = null;
                }
            }
            else if (state == FogState.UNEXPLORED)
            {
                if (FogObject == null)
                {
                    AddFogPrefabToTile(TilePrefab.FogBlack, FogState);
                }
            }
        }



        private void AddFogPrefabToTile(TilePrefab fogFab, FogState desiredState)
        {
            Assets.CreateTile(fogFab, new Vector3(Entity.X, 0.1f, Entity.Y), Quaternion.identity,
            o =>
            {
                if (FogState != desiredState) // changed while asset was loading
                {
                    GameObject.Destroy(o);
                    return;
                }
                o.transform.parent = FogContainer.transform;
                FogObject = o;
            });
        }



        /*

        public bool HasFog => _fogs.Count > 0;

        private void PlaceFogBorders()
        {
            if (!Entity.GetNeighbor(Direction.SOUTH).IsVisible()) AddMapBorderFog(Entity.X, Entity.Y - 1);
            if (Entity.GetNeighbor(Direction.NORTH).IsVisible()) AddMapBorderFog(Entity.X, Entity.Y + 1);
            if (Entity.GetNeighbor(Direction.EAST).IsVisible()) AddMapBorderFog(Entity.X + 1, Entity.Y);
            if (Entity.GetNeighbor(Direction.WEST).IsVisible()) AddMapBorderFog(Entity.X - 1, Entity.Y);
        }

        private void AddMapBorderFog(int x, int y)
        {
            if (_fogContainer == null) _fogContainer = new GameObject("FogContainer");
            _assets.CreateTile(TilePrefab.FogBlack, new Vector3(x, 0.1f, y), Quaternion.Euler(0, 0, 0), f => f.transform.parent = _fogContainer.transform);
        }

        

        public void SetFogInTileView(bool unexplored, bool seenBefore)
        {
            _withFog = unexplored;
            if (unexplored)
            {
                if (_fogContainer == null) _fogContainer = new GameObject("FogContainer");
                var prefab = seenBefore ? TilePrefab.Fog50 : TilePrefab.FogBlack;
                AddFogPrefabToTile(prefab);
            }
            else if (_fogs.Count > 0)
            {
                foreach (var f in _fogs) GameObject.Destroy(f);
                _fogs.Clear();
            }
        }
        */

        public override string ToString() => $"<TileView {Entity}>";
    }
}