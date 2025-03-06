using Game.Engine.ECLS;
using Game.Systems.Battler;
using GameData.Specs;

namespace Game.Systems.Dungeon
{
    public class DungeonLogic : BaseEntityLogic<DungeonComponent>
    {
        public void SetUnitsFromSpec(DungeonSpec spec)
        {
            var group = CurrentEntity.Components.Get<BattleGroupComponent>();
            for (var s = 0; s < spec.BattleSpecs.Count; s++)
            {
                var battle = spec.BattleSpecs[s];
                var units = new Unit[battle.UnitSpecIDS.Length];
                for (var i = 0; i < battle.UnitSpecIDS.Length; i++)
                {
                    units[i] = new Unit(CurrentEntity.Game.Specs.Units[battle.UnitSpecIDS[i]]);
                    group.Units.Add(units[i]);
                }
            }
            CurrentEntity.Save(group);
            var dg = CurrentEntity.Components.Get<DungeonComponent>();
            dg.SpecId = spec.SpecId;
            CurrentEntity.Save(dg);
        }
    }
}
