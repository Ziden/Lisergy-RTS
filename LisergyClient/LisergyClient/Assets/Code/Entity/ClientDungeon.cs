using Assets.Code.Views;
using Game;
using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientDungeon : Dungeon, IGameObject, IClientEntity<Dungeon, ClientDungeon>
    {
        public GameObject GameObject { get; set; }

        public ClientDungeon UpdateData(Dungeon dungeon)
        {
            this._battles = dungeon.Battles;
            this.Id = dungeon.Id;
            this.Tile = GameView.World.GetTile(dungeon);
            return this;
        }

        public void InstantiateInScene(Dungeon dungeon)
        {
            var prefab = Resources.Load("prefabs/buildings/dungeon");
            var tile = GameView.World.GetTile(dungeon);
            var chunkView = GameView.GetView<ChunkView>(tile.Chunk);
            GameObject = MainBehaviour.Instantiate(prefab, chunkView.GameObject.transform) as GameObject;
            UpdateData(dungeon);
        }

        public ClientDungeon(PlayerEntity owner) 
        {
         
        }
    }
}
