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
            Assets.CreatePrefab(spec.Art, Vector3.zero, Quaternion.identity, o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.Tile);
                GameObject = o;
                tileView.SetChildren(GameObject);
                GameObject.transform.localPosition = Vector3.zero;
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
            });
        }
    }
}
