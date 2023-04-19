using Game;
using Game.ECS;
using Game.Events.GameEvents;
using System;
using UnityEngine;

namespace Assets.Code.Views
{
    public class ChunkView : IEntityView
    {
        private static GameObject _chunksNode;

        public bool NeedsInstantiate => _chunksNode != null;

        public Chunk Entity { get; }
        public GameObject GameObject { get; set; }

        IEntity IEntityView.Entity => Entity;

        public ChunkView(ref Chunk c) : base()
        {
            Entity = c;
            InstantiationImplementation();
        }

        public void InstantiationImplementation()
        {
            if (_chunksNode == null)
            {
                _chunksNode = new GameObject("Chunks");
                _chunksNode.isStatic = true;
            }
            GameObject = new GameObject($"Chunk-{Entity.X}-{Entity.Y}");
            GameObject.transform.SetParent(_chunksNode.transform);
            GameObject.transform.position = new Vector3(Entity.X * GameWorld.CHUNK_SIZE, Entity.Y * GameWorld.CHUNK_SIZE);
        }
    }
}
