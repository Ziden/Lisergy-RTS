using System.Threading.Tasks;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Game.Systems.Map;
using LisergyGodotClient.Src.Systems.Animation;

namespace LisergyGodotClient.Src.Systems.Movement;

public class MovablevView(IEntity entity, IClientSdk client)
	: GodotSpriteEntityView(entity, client), IEntityMovementInterpolated
{
	public MovementInterpolatorLogic MovementInterpolator => new(Client, Entity);

	protected override async Task CreateView()
	{
		var data = Entity.Get<BattleGroupComponent>();
		var placed = Entity.Get<MapPlacementComponent>();
		var spec = Client.Game.Specs.Units[data.Units[0].SpecId];
		GameObject = await ClientServices.Assets.LoadGetArt(spec.Art);
		ClientServices.Assets.AddToScene(GameObject, placed.Position);
	}
}