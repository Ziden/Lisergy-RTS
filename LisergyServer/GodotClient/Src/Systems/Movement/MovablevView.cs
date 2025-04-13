using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.World;
using Godot;
using Godot.Collections;
using LisergyGodotClient.Src.Systems.Animation;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Systems.Movement
{
    public class MovablevView : GodotSpriteEntityView, IEntityMovementInterpolated
    {
        public MovablevView(IEntity entity, IClientSdk client) : base(entity, client)
        {
        }

        public MovementInterpolatorLogic MovementInterpolator => new MovementInterpolatorLogic(Client, Entity);

        protected override async Task CreateView()
        {
            var data = Entity.Get<BattleGroupComponent>();
            var placed = Entity.Get<MapPlacementComponent>();
            var spec = Client.Game.Specs.Units[data.Units[0].SpecId];
            GameObject = await ClientServices.Assets.LoadGetArt(spec.Art);
            ClientServices.Assets.AddToScene(GameObject, placed.Position);
        }

     
    }
}
