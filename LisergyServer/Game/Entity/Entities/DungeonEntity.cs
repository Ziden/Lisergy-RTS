using Game.Battles;
using Game.Entity.Components;
using Game.Entity.Logic;
using Game.Events;
using Game.Events.GameEvents;
using Game.World.Systems;
using System;
using System.Linq;

namespace Game.Entity.Entities
{
    [Serializable]
    public class DungeonEntity : WorldEntity, IBattleableEntity
    {
        [NonSerialized]
        private BattleComponentsLogic _battleLogic;

        public DungeonEntity(params Unit[] units) : base(Gaia)
        {
            InitComponents();
            _battleLogic.UpdateUnits(units.ToList());
        }

        public DungeonEntity(ushort specId) : base(Gaia)
        {
            InitComponents();
            BuildFromSpec(specId);
        }

        public DungeonEntity() : base(Gaia)
        {
            InitComponents();
        }

        public DungeonEntity(PlayerEntity e) : base(e)
        {
            InitComponents();
        }

        public IBattleComponentsLogic BattleLogic => _battleLogic;

        public ushort SpecID => Components.Get<DungeonComponent>().SpecId;

        public override string ToString()
        {
            return $"<Dungeon Spec={SpecID}/>";
        }

        public void BuildFromSpec(ushort specID)
        {
            var spec = StrategyGame.Specs.Dungeons[specID];
            for (var s = 0; s < spec.BattleSpecs.Count; s++)
            {
                var battle = spec.BattleSpecs[s];
                var units = new Unit[battle.UnitSpecIDS.Length];
                for (var i = 0; i < battle.UnitSpecIDS.Length; i++)
                {
                    var unitSpecID = battle.UnitSpecIDS[i];
                    var unitSpec = StrategyGame.Specs.Units[unitSpecID];
                    units[i] = new Unit(unitSpecID);
                    units[i].SetSpecStats();
                    BattleLogic.AddUnit(units[i]);
                }
                if(s < spec.BattleSpecs.Count - 1)
                {
                    BattleLogic.NewUnitLine();
                }
            }
            Components.Get<DungeonComponent>().SpecId = specID;
        }

        private void InitComponents()
        {
            this.Components.Add(new DungeonComponent());
            this.Components.Add(new BattleGroupComponent());
            this.Components.Add(new BuildingComponent());
            _battleLogic = new BattleComponentsLogic(this);
        }
        public ServerPacket GetStatusUpdatePacket() => UpdatePacket;
    }
}
