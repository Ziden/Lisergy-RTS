using Game.DataTypes;
using Game.ECS;
using Game.Systems.BattleGroup;
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
        public UnitGroup Units;

        public override string ToString() => $"<BattleGroup Units={Units} BattleId={BattleID}>";
    }
}
