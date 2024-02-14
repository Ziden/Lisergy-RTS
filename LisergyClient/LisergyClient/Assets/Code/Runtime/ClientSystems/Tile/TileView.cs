using Game.Tile;
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
            State = EntityViewState.RENDERING;
            var tileSpec = Client.Game.Specs.Tiles[Entity.SpecId];
            Assets.CreatePrefab(tileSpec.TilePrefab, new Vector3(Entity.X, 0, Entity.Y), Quaternion.identity, o =>
            {
                GameObject = o;
                GameObject.transform.parent = GetChunkObject().transform;
                GameObject.name = $"Tile_{Entity.X}-{Entity.Y}";
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
                Client.ClientEvents.Call(new TileRenderedEvent() { View = this, Reactivate = false });
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
                GameObject?.SetActive(true);
                if (FogObject != null)
                {
                    GameObject.Destroy(FogObject);
                    FogObject = null;
                }
            }
            else if (state == FogState.UNEXPLORED)
            {
                GameObject?.SetActive(false);
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
                if (FogState != desiredState || FogObject != null) // changed while asset was loading
                {
                    Debug.Log($"Invalid fog created for {Entity}");
                    GameObject.Destroy(o);
                    return;
                }
                o.name = $"Fog for {Entity.X} {Entity.Y}";
                o.transform.parent = FogContainer.transform;
                FogObject = o;
            });
        }

        public override string ToString() => $"<TileView {Entity}>";
    }
}