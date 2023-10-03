using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Battler
{
    public interface IBattleGroupComponentData
    {
        public GameId BattleID { get; }
        public IReadOnlyList<Unit> GroupUnits { get; }
        public bool IsDestroyed { get; }
    }

    [Serializable]
    [SyncedComponent()]

    public class BattleGroupComponent : IComponent, IBattleGroupComponentData
    {
        public GameId BattleID { get; set; }
        public List<Unit> Units { get; set; } = new List<Unit>();
        public bool IsDestroyed => Units.All(u => u == null || u.HP <= 0);
        public IReadOnlyList<Unit> GroupUnits => Units;
    }
}
