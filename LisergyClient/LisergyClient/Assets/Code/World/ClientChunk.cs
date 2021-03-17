
using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientChunk : Chunk, IGameObject
    {
        private static GameObject _chunksNode;

        private GameObject _gameObject;

        public GameObject GetGameObject() => _gameObject;

        public ClientChunk(ClientChunkMap chunkMap, int x, int y): base(chunkMap, x, y, new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE])
        {
            if (_chunksNode == null)
                _chunksNode = new GameObject("Chunks");
            _gameObject = new GameObject($"Chunk-{x}-{y}");
            _gameObject.transform.SetParent(_chunksNode.transform);
            _gameObject.transform.position = new Vector3(x * GameWorld.CHUNK_SIZE, y * GameWorld.CHUNK_SIZE);
        }

       
    }
}
