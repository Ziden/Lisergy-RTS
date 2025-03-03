using Game.Engine.ECLS;
using System;

namespace Game.Systems.DeltaTracker
{
    public enum DeltaFlag : byte
    {
        COMPONENTS = 1 << 1, // entity updated its components
        CREATED = 1 << 2,   // entity is created or destroyed 
        SELF_REVEALED = 1 << 3,   // entity is revealed - should only sent to triggerer 
        SELF_CONCEALED = 1 << 4   // entity is concealed - should only sent to triggerer 
    }

    [Serializable]
    public class NetworkedComponent : IComponent
    {
    }

    [Serializable]
    public class DeltaFlagsComponent : IComponent
    {
        public DeltaFlag Flags;

        public bool HasFlag(DeltaFlag f) => Flags.HasFlag(f);

        public bool SetFlag(DeltaFlag f)
        {
            var hasDelta = f > 0 && Flags == 0;
            Flags |= f;
            return hasDelta;
        }

        public bool HasFlags() => Flags != 0;

        public void Clear() { Flags = 0; }

        public override string ToString() => Convert.ToString((byte)Flags, 2).PadLeft(8, '0');
    }
}
