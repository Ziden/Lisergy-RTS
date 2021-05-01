using Game.Battles.Actions;
using Game.BattleTactics;
using Game.TacticsBattle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Battles
{
    public class TacticsBattle
    {
        protected List<BattleTeam> _teams = new List<BattleTeam>();
        protected TacticsMap _map;

        public Dictionary<string, BattleUnit> _units = new Dictionary<string, BattleUnit>();
        private List<BattleAction> _log = new List<BattleAction>();
     
        public BattleUnit [] Units { get => _units.Values.ToArray(); }
        public BattleTeam[] Teams { get => _teams.ToArray(); }

        public string ID { get; private set; }
        public SortedSet<BattleUnit> ActionQueue { get; } = new SortedSet<BattleUnit>();

        public BattleUnit CurrentUnitTurn { get => ActionQueue.First(); }

        public long RTElapsed = 0;

        public TacticsBattle(TacticsMap map)
        {
            ID = Guid.NewGuid().ToString();
            this._map = map;
        }

        public BattleUnit GetUnit(string id)
        {
            return _units[id];
        }

        public void MoveUnitTo(BattleUnit battleUnit, int x, int y)
        {
            if (battleUnit.Team == null) throw new Exception("Battle Unit needs a team to join battle");
            if (!_teams.Contains(battleUnit.Team))
                _teams.Add(battleUnit.Team);
            _units[battleUnit.UnitID] = battleUnit;
            ActionQueue.Add(battleUnit);
            battleUnit.Tile = this._map.GetTacticsTile(x, y);
        }

        public void PassTurn()
        {
            var unitPassedTurn = CurrentUnitTurn;
            var crt = CurrentUnitTurn.RT;
            unitPassedTurn.RT += unitPassedTurn.GetMaxRT();
            ActionQueue.Remove(unitPassedTurn);
            ActionQueue.Add(unitPassedTurn);
            RTElapsed += crt = CurrentUnitTurn.RT;
        }

        public bool ReceiveIntent(BattleAction action)
        {
            if(action.Unit != CurrentUnitTurn)
            {
                Log.Error($"Not unit's turn {action.Unit} - current turn is {CurrentUnitTurn}");
                action.Result = new ActionResult() { Succeeded = false };
                return false;
            }

            if (action is MoveAction)
            {
                var result = new ActionResult() { Succeeded = false };
                action.Result = result;
                var unit = action.Unit;
                if(unit.Moved)
                {
                    Log.Error($"{unit} already moved");
                    result.Succeeded = false;
                    return false;
                }
                var move = (MoveAction)action;
                var destTile = _map.GetTile(move.TileX, move.TileY);
                var pathToDest = _map.FindPath(unit.Tile, _map.GetTile(move.TileX, move.TileY));
                if(pathToDest == null || pathToDest.Count == 0 || pathToDest.Count > unit.Stats.Move)
                {
                    Log.Error($"Cannot move {unit} to {destTile}");
                    result.Succeeded = false;
                    return result.Succeeded;
                }
                unit.IncreaseRT(pathToDest.Count * BattleUnit.RT_PER_TILE_MOVED);
                MoveUnitTo(unit, move.TileX, move.TileY);
                unit.Moved = true;
                result.Succeeded = true;
                _log.Add(action);
                return result.Succeeded;
            }
            return false;
        }
    }
}
