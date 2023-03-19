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
            var tileView = GameView.GetView(tile.Chunk);
            GameObject = MainBehaviour.Instantiate(prefab, tileView.GameObject.transform) as GameObject;
            this.Tile = GameView.World.GetTile(dungeon);
            GameObject.transform.position = new Vector3(Tile.X, 0, tile.Y);
            Debug.Log("Instantiated dungeon");
        }

        public ClientDungeon(PlayerEntity owner) 
        {
         
        }
    }
}
