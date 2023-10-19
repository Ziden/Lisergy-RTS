using Assets.Code.Views;
using ClientSDK.Data;
using Game.Systems.Dungeon;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : UnityEntityView<DungeonEntity>
    {
        protected override void CreateView()
        {
            var spec = Client.Game.Specs.Dungeons[Entity.SpecId];
            State = EntityViewState.RENDERING;
            Assets.CreatePrefab(spec.Art, new Vector3(Entity.Tile.X, 0, Entity.Tile.Y), Quaternion.identity, o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.Tile);
                GameObject = o;
                tileView.SetChildren(GameObject);
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
            });
        }
    }
}
