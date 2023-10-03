using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Systems.Player;
using GameData.Specs;
using System;
using System.Linq;

namespace Game.Systems.Dungeon
{
    [Serializable]
    public class DungeonEntity : BaseEntity, IBuildableEntity<DungeonSpec>
    {
        public DungeonEntity() : base(null)
        {
            Components.Add(new DungeonComponent());
            Components.Add(new BattleGroupComponent());
            Components.Add(new BuildingComponent());
        }

        public ushort SpecID => Components.Get<DungeonComponent>().SpecId;

        public override string ToString()
        {
            return $"<Dungeon Spec={SpecID}/>";
        }

        public ServerPacket GetStatusUpdatePacket() => GetUpdatePacket(null);

        public void BuildFromSpec(DungeonSpec spec)
        {
            var battleGroup = Components.Get<BattleGroupComponent>();
            for (var s = 0; s < spec.BattleSpecs.Count; s++)
            {
                var battle = spec.BattleSpecs[s];
                var units = new Unit[battle.UnitSpecIDS.Length];
                for (var i = 0; i < battle.UnitSpecIDS.Length; i++)
                {
                    var unitSpecID = battle.UnitSpecIDS[i];
                    units[i] = new Unit(unitSpecID);
                    units[i].SetBaseStats();
                    battleGroup.Units.Add(units[i]);
                }
            }
            Components.Get<DungeonComponent>().SpecId = spec.DungeonSpecID;
        }
    }
}
