using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientDungeon : Dungeon, IGameObject
    {
        public GameObject GetGameObject() => _gameObject;

        private GameObject _gameObject;

        public ClientDungeon(Dungeon dungeon, ClientTile tile)
        {
            var prefab = Resources.Load("prefabs/buildings/dungeon");
            _gameObject = MainBehaviour.Instantiate(prefab, ((ClientChunk)tile.Chunk).GetGameObject().transform) as GameObject;
            this.SetBattles(dungeon.Battles);
            this.Id = dungeon.Id;
        }
    }
}
