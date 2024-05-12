using Assets.Code.Assets.Code.Assets;
using Assets.Code.Views;
using ClientSDK.Data;
using Game;
using Game.Systems.Building;
using Game.Systems.Map;
using UnityEngine;

namespace Assets.Code.World
{
    public class PlayerBuildingView : UnityEntityView<PlayerBuildingEntity>
    {

        protected override void CreateView()
        {
            var spec = Client.Game.Specs.Buildings[Entity.SpecId];
            State = EntityViewState.RENDERING;
            UnityServicesContainer.Resolve<IAssetService>().CreatePrefab(spec.Art, new Vector3(Entity.Tile.X, 0, Entity.Tile.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.Tile);
                GameObject = o;
                GameObject.name = $"Building {Entity.EntityId} from {Entity.OwnerID}";
                GameObject.isStatic = true;
                tileView.SetChildren(GameObject);
                State = EntityViewState.RENDERED;
            });
        }
    }
}
