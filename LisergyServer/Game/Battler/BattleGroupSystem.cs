using Game.ECS;

namespace Game.Battler
{
    public class BattleGroupSystem : GameSystem<BattleGroupComponent, WorldEntity>
    {
        public delegate void Test(IEntity entity);

        internal override void OnComponentAdded(WorldEntity owner, BattleGroupComponent component, EntityEventBus events)
        {

        }


    }
}
