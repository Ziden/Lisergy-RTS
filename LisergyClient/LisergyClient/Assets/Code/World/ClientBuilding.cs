using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientBuilding : Building
    {
        public GameObject GameObject;

        public ClientBuilding(byte id, ClientPlayer owner, ClientTile tile): base(id, owner)
        {
            var prefab = Resources.Load("prefabs/buildings/"+id);
            StackLog.Debug("Instantiating BUILDING");
            GameObject = MainBehaviour.Instantiate(prefab, ((ClientChunk)tile.Chunk).ChunkObject.transform) as GameObject;
        }

        public bool IsMine()
        {
            return Owner == MainBehaviour.Player;
        }
    }
}
