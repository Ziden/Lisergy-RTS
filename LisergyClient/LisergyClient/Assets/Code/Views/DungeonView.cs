using Assets.Code.Entity;
using Assets.Code.Views;
using Game;
using Game.ECS;
using Game.Entity.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : EntityView<DungeonEntity>
    {
        public override bool Instantiated => GameObject != null;
        public override DungeonEntity Entity { get; }
        public override GameObject GameObject { get; set; }

        public DungeonView(DungeonEntity e)
        {
            Entity = e;
        }

        public override void OnUpdate(DungeonEntity serverEntity, List<IComponent> syncedComponents)
        {
            Entity.Tile = GameView.World.GetTile(serverEntity);
            if (!Instantiated)
            {
                Instantiate();
            }
        }

        public override void Instantiate()
        {
            var prefab = Resources.Load("prefabs/buildings/dungeon");
            var tileView = GameView.GetOrCreateTileView(Entity.Tile, true);
            GameObject = MainBehaviour.Instantiate(prefab, tileView.GameObject.transform) as GameObject;
            GameObject.transform.position = new Vector3(tileView.Entity.X, 0, tileView.Entity.Y);
            Debug.Log("Instantiated dungeon");
        }
    }
}
