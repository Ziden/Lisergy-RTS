using Game.Engine.ECLS;
using Game.Systems.Battler;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Castle
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    [SyncedComponent]
    public struct TavernComponent : IComponent
    {
        public byte Level;
        public Unit Bartender;
        public TavernTable Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct TavernTable
    {
        public Unit Chair1;
        public Unit Chair2;
        public Unit Chair3;
        public Unit Chair4;
    }
}
