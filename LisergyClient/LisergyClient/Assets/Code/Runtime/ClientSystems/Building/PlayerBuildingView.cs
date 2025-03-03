using Assets.Code.Assets.Code.Assets;
using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game.Entities;
using Game.Engine.ECLS;
using Game.Systems.Building;
using Game.Systems.Map;
using UnityEngine;

namespace Assets.Code.World
{
    public class PlayerBuildingView : UnityEntityView
    {
        public PlayerBuildingView(IGameClient client, IEntity e) : base(e, client) { }
        protected override void CreateView()
        {
            var spec = Client.Game.Specs.Buildings[Entity.Get<PlayerBuildingComponent>().SpecId];
            State = EntityViewState.RENDERING;
            var tile = Entity.GetTile();
            UnityServicesContainer.Resolve<IAssetService>().CreatePrefab(spec.Art, new Vector3(tile.X, 0, tile.Y), Quaternion.Euler(0, 0, 0), o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(tile.TileEntity);
                GameObject = o;
                GameObject.name = $"Building {Entity.EntityId} from {Entity.OwnerID}";
                GameObject.isStatic = true;
                tileView.SetChildren(GameObject);
                State = EntityViewState.RENDERED;
            });
        }
    }
}
