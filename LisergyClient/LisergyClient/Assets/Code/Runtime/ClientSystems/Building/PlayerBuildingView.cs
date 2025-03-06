using Assets.Code.Assets.Code.Assets;
using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Building;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.World
{
    public class PlayerBuildingView : UnityEntityView
    {
        public PlayerBuildingView(IGameClient client, IEntity e) : base(e, client) { }
        protected override async Task CreateView()
        {
            var spec = Client.Game.Specs.Buildings[Entity.Get<PlayerBuildingComponent>().SpecId];
            State = EntityViewState.RENDERING;
            var tile = Entity.GetTile();
            var o = await UnityServicesContainer.Resolve<IAssetService>().CreatePrefab(spec.Art, new Vector3(tile.X, 0, tile.Y));
            var tileView = (TileView)Client.Modules.Views.GetOrCreateView(tile.Entity);
            GameObject = o;
            GameObject.name = $"Building {Entity.EntityId} from {Entity.OwnerID}";
            tileView.SetChildren(GameObject);
        }
    }
}
