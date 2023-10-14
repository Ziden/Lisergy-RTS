using Assets.Code.Assets.Code.Assets;
using Assets.Code.Entity;
using Assets.Code.Views;
using ClientSDK.Data;
using Game;
using Game.ECS;
using Game.Systems.Dungeon;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : EntityView<DungeonEntity>
    {
        protected void InstantiationImplementation()
        {
            /*
            var dgs = StrategyGame.Specs.Dungeons;
            var id = Entity.SpecID;
            var spec = Entity.GetSpec();
            _assets.CreatePrefab(spec.Art, new Vector3(Entity.Tile.X, 0, Entity.Tile.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                var view = GameView.GetOrCreateTileView(Entity.Tile, true);
                view.OnInstantiate(tileObject =>
                {
                    o.transform.parent = tileObject.transform;
                    SetGameObject(o);
                });
            });
            Debug.Log($"Instantiated dungeon at {Entity.Tile}");
            */
        }
    }
}
