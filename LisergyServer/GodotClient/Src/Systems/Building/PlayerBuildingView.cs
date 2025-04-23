using System.Threading.Tasks;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.Building;
using Game.Systems.Map;
using GameData.Specs;

namespace LisergyGodotClient.Src.Systems;

public class PlayerBuildingView(IEntity entity, IClientSdk client) : EntityView(entity, client)
{
	private bool _isConstructionSite;

	protected override async Task CreateView()
	{
		var building = Entity.Get<PlayerBuildingComponent>();
		ArtSpec art = null;
		_isConstructionSite = Entity.Logic.Building.IsConstruction();
		if (!_isConstructionSite)
		{
			art = Client.Game.Specs.Buildings[building.SpecId].Art;
		}
		else
		{
			art = AssetConfigs.TILE_CONSTRUCTION_SITE;
		}
		GameObject = await ClientServices.Assets.LoadGetArt(art);
		ClientServices.Assets.AddToScene(GameObject, Entity.Get<MapPlacementComponent>().Position);
	}
}