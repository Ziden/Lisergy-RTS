using Game.Network;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Map;
using Game.Systems.MapPosition;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace Game.Systems.Dungeon
{
    public class DungeonEntity : BaseEntity
    {
        public override EntityType EntityType => EntityType.Dungeon;

        public DungeonEntity(IGame game) : base(game, null)
        {
            Components.Add(new BuildingComponent());
            Components.Add(new MapPlacementComponent());
            Components.Add(new MapReferenceComponent());
            Components.Add(new DungeonComponent());
            Components.Add(new BattleGroupComponent()
            {
                Units = new List<Unit>()
            });
        }

        public ushort SpecId => Components.Get<DungeonComponent>().SpecId;

        public override string ToString()
        {
            return $"<Dungeon Spec={SpecId}/>";
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
                    units[i] = new Unit(Game.Specs.Units[battle.UnitSpecIDS[i]]);
                    battleGroup.Units.Add(units[i]);
                }
            }
            var component = Components.Get<DungeonComponent>();
            component.SpecId = spec.DungeonSpecID;
            Components.Save(component);
        }
    }
}
