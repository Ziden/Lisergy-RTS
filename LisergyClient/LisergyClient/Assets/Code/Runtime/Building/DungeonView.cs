using Assets.Code.Assets.Code.Assets;
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
        private IAssetService _assets;

        public DungeonView(DungeonEntity e)
        {
            Entity = e;
            _assets = ServiceContainer.Resolve<IAssetService>();
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
            var dgs = StrategyGame.Specs.Dungeons;
            var id = Entity.SpecID;

            var spec = Entity.GetSpec();
            _assets.CreatePrefab(spec.Art, new Vector3(Entity.Tile.X, 0, Entity.Tile.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                var view = GameView.GetView(Entity.Tile.Chunk);
                o.transform.parent = view.GameObject.transform;
            });
            Debug.Log($"Instantiated dungeon at {Entity.Tile}");
        }
    }
}
