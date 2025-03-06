using Game.Engine.ECLS;
using Game.World;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Building
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public class BuilderComponent : IComponent
    {
        public Location BuildingAt;

        public override string ToString()
        {
            return $"<Builder>";
        }
    }
}
