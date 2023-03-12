using Game;
using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientDungeon : Dungeon, IGameObject, IClientEntity<Dungeon, ClientDungeon>
    {
        private GameObject _gameObject;

        public GameObject GetGameObject() => _gameObject;

        public ClientDungeon UpdateData(Dungeon dungeon)
        {
            this._battles = dungeon.Battles;
            this.Id = dungeon.Id;
            this.Tile = ClientStrategyGame.ClientWorld.GetClientTile(dungeon);
            return this;
        }

        public void InstantiateInScene(Dungeon dungeon)
        {
            var prefab = Resources.Load("prefabs/buildings/dungeon");
            var tile = ClientStrategyGame.ClientWorld.GetClientTile(dungeon);
            _gameObject = MainBehaviour.Instantiate(prefab, tile.ClientChunk.GetGameObject().transform) as GameObject;
            UpdateData(dungeon);
        }

        public ClientDungeon(PlayerEntity owner) 
        {
         
        }
    }
}
