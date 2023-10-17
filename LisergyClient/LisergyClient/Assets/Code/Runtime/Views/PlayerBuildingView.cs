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
            GameObject = new GameObject($"Building {Entity.EntityId} from {Entity.OwnerID}");
            GameObject.transform.parent = ViewContainer.transform;
            State = EntityViewState.RENDERING;
            ClientServices.Resolve<IAssetService>().CreatePrefab(spec.Art, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.Tile);
                o.isStatic = true;
                o.transform.parent = GameObject.transform;
                o.transform.position = Entity.UnityPosition();
                State = EntityViewState.RENDERED;
            });
        }
    }
}
