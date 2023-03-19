using Game.Battle;
using Game.Battles;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity
{
    [Serializable]
    public class Dungeon : BuildingEntity, IBattleable
    {
        private ushort DungeonSpecID;

        protected List<Unit[]> _battles = new List<Unit[]>();
        private Item[] _rewards;

        public List<Unit[]> Battles { get => _battles; }
        public Item[] Rewards { get => _rewards; set => _rewards = value; }

        public bool IsBattling => throw new NotImplementedException();

        public Dungeon(): base(Gaia) {}

        public bool IsComplete()
        {
            return !_battles.Any(battle => battle.Any(unit => unit.Stats.HP != 0));
        }

        public Dungeon(params Unit[] fights): base(Gaia)
        {
            foreach (var unit in fights)
                unit.SetSpecStats();
            this._battles.Add(fights);
        }

        public void AddBattle(params Unit[] units)
        {
            this._battles.Add(units);
        }

        public override string ToString()
        {
            return $"<Dungeon Spec={DungeonSpecID} battles={_battles.Count}>";
        }

        public BattleTeam GetBattleTeam()
        {
            var units = this.Battles.First();
            return new BattleTeam(this, units);
        }

        public static Dungeon BuildFromSpec(ushort specID)
        {
            var dg = new Dungeon();
            var spec = StrategyGame.Specs.Dungeons[specID];
            foreach(var battle in spec.BattleSpecs)
            {
                var units = new Unit[battle.UnitSpecIDS.Length];
                for(var i =0; i < battle.UnitSpecIDS.Length; i++)
                {
                    var unitSpecID = battle.UnitSpecIDS[i];
                    var unitSpec = StrategyGame.Specs.Units[unitSpecID];
                    units[i] = new Unit(unitSpecID);
                    units[i].SetSpecStats();
                }
                dg.Battles.Add(units);      
            }
            dg.DungeonSpecID = specID; 
            return dg;
        }

        public void OnBattleFinished(TurnBattle battle, BattleHeader BattleHeader, BattleTurnEvent[] Turns)
        {
            if (IsComplete())
            {
                this.Tile = null;
            }
        }

        public void OnBattleStarted(TurnBattle battle)
        {
            
        }

        public ServerPacket GetStatusUpdatePacket() => new EntityUpdatePacket(this);
    }
}
