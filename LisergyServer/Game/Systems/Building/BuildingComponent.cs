using Game.ECS;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Building
{
    /// <summary>
    /// Represents something that can be placed statically in the map
    /// Only one static can be in a given tile at a time.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct BuildingComponent : IComponent
    {
        public override string ToString()
        {
            return $"<BuildingComponent>";
        }
    }
}
