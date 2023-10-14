using Game.DataTypes;
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

        public DungeonEntity(IGame game) : base(game, GameId.ZERO)
        {
            Components.Add<DungeonComponent>();
            Components.Add<BuildingComponent>();
            Components.Add<MapPlacementComponent>();
            Components.Add<BattleGroupComponent>();
            Components.AddReference(new MapReferenceComponent());
        }

        public ushort SpecId => Components.Get<DungeonComponent>().SpecId;

        public override string ToString()
        {
            return $"<Dungeon Entity={EntityId} Spec={SpecId} Units={Components.Get<BattleGroupComponent>().Units}/>";
        }

        public unsafe void BuildFromSpec(DungeonSpec spec)
        {
            var group = Components.GetPointer<BattleGroupComponent>();
            for (var s = 0; s < spec.BattleSpecs.Count; s++)
            {
                var battle = spec.BattleSpecs[s];
                var units = new Unit[battle.UnitSpecIDS.Length];
                for (var i = 0; i < battle.UnitSpecIDS.Length; i++)
                {
                    units[i] = new Unit(Game.Specs.Units[battle.UnitSpecIDS[i]]);
                    group->Units.Add(units[i]);
                }
            }
            Components.GetPointer<DungeonComponent>()->SpecId = spec.DungeonSpecID;
        }
    }
}
