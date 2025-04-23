using System.Threading.Tasks;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Game.Systems.Dungeon;
using Game.Systems.Map;

namespace LisergyGodotClient.Src.Systems;

public class DungeonView(IEntity entity, IClientSdk client) : EntityView(entity, client)
{
	protected override async Task CreateView()
	{
		var position = Entity.Get<MapPlacementComponent>().Position;
		var data = Entity.Get<BattleGroupComponent>();
		var dg = Entity.Get<DungeonComponent>();
		var spec = Client.Game.Specs.Dungeons[dg.SpecId];
		GameObject = await ClientServices.Assets.LoadGetArt(spec.Art);
		ClientServices.Assets.AddToScene(GameObject, position);
	}
}