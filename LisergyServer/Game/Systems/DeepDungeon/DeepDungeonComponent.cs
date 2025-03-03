using Game.Engine.ECLS;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Castle
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    [SyncedComponent]
    public class DeepDungeonComponent : IComponent
    {
        public ushort DungeonLevel;
    }
}
