using Assets.Code.Views;
using Game;
using Game.ECS;
using Game.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientDungeon : Dungeon, IGameObject, IClientEntity<Dungeon, ClientDungeon>
    {
        public GameObject GameObject { get; set; }

        public ClientDungeon UpdateData(Dungeon dungeon, List<IComponent> syncedComponents)
        {
            this._battles = dungeon.Battles;
            this.Id = dungeon.Id;
            if(GameObject != null)
                this.Tile = GameView.World.GetTile(dungeon);
            return this;
        }

        public void InstantiateInScene(Dungeon dungeon)
        {
            var prefab = Resources.Load("prefabs/buildings/dungeon");
            var tile = GameView.World.GetTile(dungeon);
            var chunkView = GameView.GetView<ChunkView>(tile.Chunk);
            GameObject = MainBehaviour.Instantiate(prefab, chunkView.GameObject.transform) as GameObject;
            this.Tile = GameView.World.GetTile(dungeon);
        }

        public ClientDungeon(PlayerEntity owner) 
        {
         
        }
    }
}
