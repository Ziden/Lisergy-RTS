using Game;
using Game.Events.GameEvents;
using System;
using UnityEngine;

namespace Assets.Code.Views
{
    public class ChunkView : IEntityView
    {
        private static GameObject _chunksNode;

        public Chunk Chunk;
        public GameObject GameObject { get; set; }

        public ChunkView(Chunk c) : base()
        {
            Chunk = c;
            if (_chunksNode == null)
                _chunksNode = new GameObject("Chunks");
            GameObject = new GameObject($"Chunk-{Chunk.X}-{Chunk.Y}");
            GameObject.transform.SetParent(_chunksNode.transform);
            GameObject.transform.position = new Vector3(Chunk.X * GameWorld.CHUNK_SIZE, Chunk.Y * GameWorld.CHUNK_SIZE);
        }
    }
}
