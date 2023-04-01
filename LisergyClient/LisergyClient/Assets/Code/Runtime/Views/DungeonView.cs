using Assets.Code.Entity;
using Assets.Code.Views;
using Game;
using Game.Dungeon;
using Game.ECS;
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
            var view = GameView.GetView(Entity.Tile.Chunk);
            GameObject = MainBehaviour.Instantiate(prefab, view.GameObject.transform) as GameObject;
            GameObject.transform.position = new Vector3(Entity.Tile.X, 0, Entity.Tile.Y);
            Debug.Log("Instantiated dungeon");
        }
    }
}
