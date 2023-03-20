using Game.Battler;
using Game.Building;
using Game.Events;
using Game.Player;
using System;
using System.Linq;

namespace Game.Dungeon
{
    [Serializable]
    public class DungeonEntity : WorldEntity, IBattleableEntity
    {
        [NonSerialized]
        private BattleGroupComponentLogic _battleLogic;

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

        public IBattleComponentsLogic BattleGroupLogic => _battleLogic;

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
                    BattleGroupLogic.AddUnit(units[i]);
                }
                if (s < spec.BattleSpecs.Count - 1)
                {
                    BattleGroupLogic.NewUnitLine();
                }
            }
            Components.Get<DungeonComponent>().SpecId = specID;
        }

        private void InitComponents()
        {
            Components.Add(new DungeonComponent());
            Components.Add(new BattleGroupComponent());
            Components.Add(new BuildingComponent());
            _battleLogic = new BattleGroupComponentLogic(this);
        }
        public ServerPacket GetStatusUpdatePacket() => UpdatePacket;
    }
}
