using ClientSDK;
using Game.ECS;
using Game.Engine.ECS;
using Game.Systems.Battler;
using Game.Systems.Party;



public class BattleGroupUnitListener : BaseComponentListener<BattleGroupComponent>
{
    public BattleGroupUnitListener(IGameClient client) : base(client)
    {
    }

    public override void OnUpdateComponent(IEntity entity, BattleGroupComponent oldComponent, BattleGroupComponent newComponent)
    {
        if (!(entity is PartyEntity party)) return;
        if (oldComponent.Units.Equals(newComponent.Units)) return;
        party.GetEntityView().UpdateUnits(newComponent.Units);
    }
}