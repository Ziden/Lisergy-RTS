using Game.Engine.ECLS;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Component added to entities that can harvest resources
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [RequiresComponent(typeof(CargoComponent))]
    public class HarvesterComponent : IComponent { }
}