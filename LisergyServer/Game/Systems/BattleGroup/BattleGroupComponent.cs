using Game.DataTypes;
using Game.ECS;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Battler
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    [SyncedComponent]
    public unsafe struct BattleGroupComponent : IComponent
    {
        public GameId BattleID;
        public List<Unit> Units;

        public override string ToString() => $"<BattleGroup Units={Units.Count} BattleId={BattleID}>";
    }
}
