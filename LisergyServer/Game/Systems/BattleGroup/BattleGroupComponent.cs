using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.BattleGroup;
using System;
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
    public struct BattleGroupComponent : IComponent
    {
        public GameId BattleID;
        public UnitGroup Units;

        public override string ToString() => $"<BattleGroup Units={Units} BattleId={BattleID}>";
    }
}
