using Game.ECS;
using Game.Entity.Entities;
using Game.World.Systems;
using System;

namespace Game.Entity.Components
{
    [SyncedComponent]
    [Serializable]
    public class PartyComponent : IComponent
    {
        /// <summary>
        /// Party index on player party array
        /// </summary>
        public byte PartyIndex;

        static PartyComponent()
        {
            SystemRegistry<PartyComponent, PartyEntity>.AddSystem(new PartySystem());
        }
    }
}
