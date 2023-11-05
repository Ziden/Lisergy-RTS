using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Party;
using GameAssets;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


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