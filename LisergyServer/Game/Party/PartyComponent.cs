using Game.ECS;
using System;

namespace Game.Party
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
