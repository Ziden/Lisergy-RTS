
using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientChunk : Chunk
    {
        private static GameObject _chunksNode;

        public GameObject ChunkObject;

        public ClientChunk(ClientChunkMap chunkMap, int x, int y): base(chunkMap, x, y, new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE])
        {
            if (_chunksNode == null)
                _chunksNode = new GameObject("Chunks");
            ChunkObject = new GameObject($"Chunk-{x}-{y}");
            ChunkObject.transform.SetParent(_chunksNode.transform);
            ChunkObject.transform.position = new Vector3(x * GameWorld.CHUNK_SIZE, y * GameWorld.CHUNK_SIZE);
        }


    }
}
