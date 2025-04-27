using Assets.Code.World;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using System.Collections.Generic;

public class BattleGroupUnitsComponent : IComponent
{
    public Dictionary<Unit, UnitView> UnitViews = new Dictionary<Unit, UnitView>();
}