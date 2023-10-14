using Game.DataTypes;
using Game.ECS;
using Game.Systems.BattleGroup;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Battler
{
    /// <summary>
    /// Refers to an entity that is a container of Units and can battle
    /// with those units.
    /// </summary>
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
