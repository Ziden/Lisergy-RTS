using Assets.Code.World;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Battler;



public class BattleGroupUnitListener : BaseComponentListener<BattleGroupComponent>
{
    public BattleGroupUnitListener(IGameClient client) : base(client)
    {
    }

    public override void OnComponentAdded(IEntity entity, BattleGroupComponent component)
    {
        if (!(entity.EntityType == EntityType.Party)) return;
        entity.GetView<PartyView>().UpdateUnits(component.Units);
    }

    public override void OnComponentModified(IEntity entity, BattleGroupComponent oldComponent, BattleGroupComponent newComponent)
    {
        if (!(entity.EntityType == EntityType.Party)) return;
        if (oldComponent.Units.Equals(newComponent.Units)) return;
        entity.GetView<PartyView>().UpdateUnits(newComponent.Units);
    }
}