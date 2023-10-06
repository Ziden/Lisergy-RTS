using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battler
{
    [Serializable]
    [SyncedComponent()]
    public class BattleGroupComponent : IComponent
    {
        public GameId BattleID { get; set; }
        public List<Unit> Units { get; set; } = new List<Unit>();

        public override string ToString()
        {
            return $"<BattleGroup UnitCount={Units.Count} BattleId={BattleID}>";
        }
    }
}
