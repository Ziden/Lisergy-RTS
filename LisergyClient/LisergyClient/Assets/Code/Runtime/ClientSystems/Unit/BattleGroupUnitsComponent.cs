using Assets.Code.World;
using Game.ECS;
using Game.Systems.Battler;
using System.Collections.Generic;

public class BattleGroupUnitsComponent : IReferenceComponent
{
    public Dictionary<Unit, UnitView> UnitViews = new Dictionary<Unit, UnitView>();
}